using System.Text.RegularExpressions;
using AutoMapper;
using Google.Cloud.TextToSpeech.V1;
using static LearnIOAPI.HttpResponses.User.UserExceptions;
using static LearnIOAPI.HttpResponses.GeneralResponses;
using LearnIOAPI.Data;
using LearnIOAPI.HttpResponses;
using LearnIOAPI.HttpResponses.User;
using LearnIOAPI.Models;
using LearnIOAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using Unsplash;

namespace LearnIOAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LanguageController: Controller
{ 
    private readonly ApplicationContext db;
    
    private static readonly UnsplashClient Unsplash= new(new ClientOptions
    {
        AccessKey = "INGzGeUNxQxM2GYA-hl_ifB96KY4VSnbwFRbBy31qXg"
    });

    private static readonly TextToSpeechClient ttsClient = TextToSpeechClient.Create();
    
    private readonly UserRepository UserRepository;

    private readonly RoutineRepository RoutineRepository;

    private readonly AssignmentRepository AssignmentRepository;

    private readonly CourseRepository CourseRepository;
    
    public LanguageController(ApplicationContext db)
    {
        this.db = db;
        AssignmentRepository = new AssignmentRepository(db);
        RoutineRepository = new RoutineRepository(db);
        UserRepository = new UserRepository(db);
        CourseRepository = new CourseRepository(db);
    }
    
    private readonly string OpenAiKey;
    
    [HttpPost("register")]
    public async Task<ObjectResult> Register(UserData userCreds)
    {
        if (await db.Users.FindAsync(userCreds.telegramID) != null)
        {
            return UserExceptions.AlreadyExists;
        }
        await UserRepository.Create(userCreds);

        return UserExceptions.Created;
    }
    
    [HttpGet("GetUser")]
    public async Task<ObjectResult> GetUser(int id)
    {
        var user = await UserRepository.Get(id);
        return user == null ? UserExceptions.NonExistent : UserExceptions.Found(user.Name);
    }
    [HttpPost("selectLanguage")]
    public async Task<ObjectResult> ChooseLanguage(int id, string language)
    {
        var user = await db.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.telegramID == id);
        
        if (user == null) return UserExceptions.NonExistent;
        
        if (user.Courses
                .Find(c => c.Language == language) != null) 
            return CourseExceptions.AlreadySigned;

        await CourseRepository.Create(id, language);

        return CourseExceptions.Added;
    }

    [HttpDelete("unregister")]
    public async Task<ObjectResult> UnRegister(int id)
    {
       var user = await UserRepository.Delete(id);
       return user ? UserExceptions.Created : UserCannotBeDeleted;
    }
    
    [HttpPost("routine")]
    public async Task<string> GenerateRoutine(int id, string language, string level)
    {
        var user = await UserRepository.Get(id);
        if (user == null) return "UserExceptions.NonExistent";
        var course = await db.Courses.FirstOrDefaultAsync(c => c.UserId == id && c.Language == language);
        
        string usedThemes;
        if (course != null)
        {
                usedThemes = string.Join(", ", await db.Routines
                .Where(routine => routine.CourseId == course.Id)
                .Select(routine => routine.Theme)
                .ToListAsync());
        }
        else
        {
            return "NonExistent(\"course\")";
        }
        var tasksText = await FillTaskGaps(language, user.MainLanguage, level, usedThemes);
        var response = await GetTasks(tasksText);
        string theme = tasksText["Theme: ".Length..];
        var tasks = SplitTasks(response);
        var taskOne = SplitTaskOneWords(tasks[0]);
        var originWords = TakeOriginWords(taskOne);
        string origins = originWords.Aggregate("", (current, word) => current + word);
        return origins;
        var taskTwo = SplitTaskTwo(tasks[1], out var taskTwoText);
        var taskThree = SplitTaskThreeOrFour(tasks[2], true);
        return originWords.ToString();
        var taskFour = SplitTaskThreeOrFour(tasks[3], false);
        var taskFive = SplitTaskFive(tasks[4], originWords);
        var TasksButFirst = new List<Dictionary<string, List<string>>>
        {
            taskTwo, taskThree, taskFour
        };
        var assignments = new List<Assignment>();

        List<byte[]> firstTaskAudio = new List<byte[]>();
        var wordsAndPictures = new List<string?>();
        foreach (var word in originWords)
        {
            wordsAndPictures.Add(await GetImage(word));
        }
        foreach (var word in originWords)
        {
            firstTaskAudio.Add(await GetTTS(word));
        }

        var taskTwoAudio = await GetTTS(taskTwoText);
        //List<Image> firstTaskImages = wordsAndPictures.Select(kvp => new Image { ImageName = kvp.Key, ImageUri = kvp.Value }).ToList();
        string picture = wordsAndPictures.Aggregate("", (current, kvp) => current + $"{kvp}\n");
        return picture;
        assignments.Add(new Assignment
        {
            Answers = taskOne.Select(kvp => kvp.Key).ToArray(),
            Task = "Learn Words",
            Questions = originWords.ToArray(),
            Audio = firstTaskAudio,
            
        });
        var questions = new List<List<string>>();
        var answers = new List<List<string>>();
        var assignmentTasks = new List<string>
        {
            "Listening Comprehension Text",
            taskThree["Text"].First(),
            taskFour["Text"].First(),
            "Incorrect Spellings"
        };
        
        
        foreach (var task in TasksButFirst)
        {
            questions.Add(task["Questions"]);
            answers.Add(task["Answers"]);
        }

        for (int i = 0; i < 4;i++)
        {
            switch (i)
            {
                case 0:
                    assignments.Add(new Assignment
                    {
                        Questions = questions.ToArray()[i].ToArray(),
                        Answers = answers.ToArray()[i].ToArray(),
                        Audio = new List<byte[]?>
                        {
                            taskTwoAudio
                        },
                        Task = assignmentTasks.ToArray()[i]

                    });
                    break;
                case 3:
                    var questionsFive = new List<string>();
                    var answerFive = new List<string>();
                    foreach (var kvp in taskFive)
                    {
                        questionsFive.Add(kvp.Key);
                        answerFive.Add(kvp.Value);
                    }
                    assignments.Add(new Assignment
                    {
                        Questions = questionsFive.ToArray(),
                        Answers = answerFive.ToArray(),
                        Task = assignmentTasks.ToArray()[i]

                    });
                    break;
                default:
                {
                    assignments.Add(new Assignment
                    {
                        Questions = questions.ToArray()[i].ToArray(),
                        Answers = answers.ToArray()[i].ToArray(),
                        Task = assignmentTasks.ToArray()[i],
                    });
                        
                    break;
                }
            }
        }
        await RoutineRepository.Create(id,language,new Routine
        {
            Theme = theme,
            Assignments = assignments,
            Completed = false,
            Mark = 0,
        });
        return "Success";
    }

    private static string[] SplitTasks(string? tasks)
    {
        return Regex.Split(tasks, @"Task \d+:")
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.Trim()) 
            .ToArray();
    }
    
    private static Dictionary<string,string> SplitTaskFive(string task, List<string> origin)
    {
        Dictionary<string, string> spellingDictionary = new Dictionary<string, string>();
        task = task.Replace("Incorrect Spellings:", "").Trim();
        Console.WriteLine(task);
        var correctWords = origin.ToArray();
        string[] lines = task.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        int minLen = Math.Min(lines.Length, correctWords.Length);

        for (int i = 0; i < minLen; i++)
        {
            string[] words = lines[i].Substring(lines[i].IndexOf(".") + 1).Split(',');
            string incorrectWords = string.Join(",", words.Select(t => t.Trim()));

            spellingDictionary[correctWords[i]] = incorrectWords;
        }

        foreach (var pair in spellingDictionary)
        {
            Console.WriteLine($"Correct word: {pair.Key}");
            Console.WriteLine("Incorrect spellings:");
            foreach (var word in pair.Value.Split(','))
            {
                Console.WriteLine(word);
            }
        }
        return spellingDictionary;
    }
    private static async Task<byte[]> GetTTS(string text)
    {
        var response = await ttsClient.SynthesizeSpeechAsync(
            new SynthesisInput { Text = text }, 
            new VoiceSelectionParams { LanguageCode = "en-US", SsmlGender = SsmlVoiceGender.Female },
            new AudioConfig { AudioEncoding = AudioEncoding.Mp3 });

        return new MemoryStream(response.AudioContent.ToByteArray()).ToArray();
    }

    private static Dictionary<string, List<string>> SplitTaskTwo(string text, out string comprehensionText)
    {
        var compTextIndex = text.IndexOf("Listening Comprehension Text:", StringComparison.Ordinal) + "Listening Comprehension Text:".Length;
        var questionsIndex = text.IndexOf("Questions:", StringComparison.Ordinal);
        var answersIndex = text.IndexOf("Answers:", StringComparison.Ordinal);

        comprehensionText = text.Substring(compTextIndex, questionsIndex - compTextIndex).Trim();
        var questions = text.Substring(questionsIndex + "Questions:".Length, answersIndex - questionsIndex - "Questions:".Length).Trim().Split('\n').Select(q => q.Trim()).ToList();
        var answers = text.Substring(answersIndex + "Answers:".Length).Trim().Split(new []{";", "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();

        var taskDictionary = new Dictionary<string, List<string>>
        {
            { "Questions", questions },
            { "Answers", answers }
        };

        return taskDictionary;
    }

    private static Dictionary<string, List<string>> SplitTaskThreeOrFour(string input, bool isThirdTask)
    {
        var questions = new List<string>();
        var result = new Dictionary<string, List<string>>();
        var splitInput = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        var taskText = isThirdTask ? splitInput[0].Split(':')[1].Trim() : splitInput[0].Replace("Grammar Exercise: ", "").Trim();
        var textList = new List<string>{taskText};
        result.Add("Text", textList);

        
        var gapFields = new List<string>();
        int i = 1;
        for (; i < splitInput.Length; i++)
        {
            if (splitInput[i].Trim() == "Answers:")
            {
                break;
            }

            var splitSentence = splitInput[i].Split(" - ")[0].Trim();
            var sentence = Regex.Replace(splitSentence, "^[0-9. ]*", "");
            gapFields.Add(sentence);
            questions = gapFields.Where(item => !string.IsNullOrEmpty(item)).ToList();
        }
        result.Add("Questions", questions);

        var correctAnswers = new List<string>();
        for (i += 1; i < splitInput.Length; i++)
        {
            var splitAnswers = splitInput[i].Split(';');
            foreach (var answer in splitAnswers)
            {
                var trimmedAnswer = answer.Trim();
                var cleanedAnswer = Regex.Replace(trimmedAnswer, "^[0-9. ]*", "");
                if (cleanedAnswer.EndsWith("."))
                {
                    cleanedAnswer = cleanedAnswer.TrimEnd('.');
                }
                correctAnswers.Add(cleanedAnswer);
            }
        }
        result.Add("Answers", correctAnswers);

        return result;
    }
    private static List<string> TakeOriginWords(Dictionary<string, string> words)
    {
        return words.Select(kvp => kvp.Key).ToList();
    }
    private static Dictionary<string, string> SplitTaskOneWords(string task1)
    {
        var lines = task1.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    
        var dictionary = lines.Select(line => Regex.Replace(line, @"^\d+\.\s*", ""))
            .Select(line => line.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
    
        return dictionary;
    }

    private static async Task<string?> GetTasks(string tasksText)
    {
        var api = new OpenAIAPI("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");
        var chat = api.Chat.CreateConversation();
        chat.AppendSystemMessage(tasksText);
        var response = await chat.GetResponseFromChatbotAsync();
        return response;
    }

    private static async Task<string> FillTaskGaps(string fromLanguage, string toLanguage, string level, string usedThemes)
    {
        var tasksText = await System.IO.File.ReadAllTextAsync("C:\\Users\\onarg\\RiderProjects\\LearnIOAPI\\NewFile1.txt");
        var replacements = new Dictionary<string, string> 
        {
            {@"\[Level\]", level},
            {@"\[Themes\]", usedThemes},
            {@"\[fromLanguage\]", fromLanguage},
            {@"\[toLanguage\]", toLanguage}
        };
        foreach(var pair in replacements)
        {
            var regex = new Regex(pair.Key);
            tasksText = regex.Replace(tasksText, pair.Value);
        }

        return tasksText;
    }
    private static async Task<string?> GetImage(string imageName)
    {
            var searchResult = await Unsplash.Search.PhotosAsync(imageName);
            var urlSmall = searchResult.Results.FirstOrDefault()?.Urls.Small;
            return urlSmall;
    }

}