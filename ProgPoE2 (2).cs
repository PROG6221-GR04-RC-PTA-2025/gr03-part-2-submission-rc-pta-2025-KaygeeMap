using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace CyberSecurityChatbot
{
    class Program
    {
        static string userName = "";
        static string favoriteTopic = "";
        static List<string> conversationHistory = new List<string>();
        static SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        static Dictionary<string, string> keywordResponses = new Dictionary<string, string>()
        {
            {"password", "🔐 Use strong, unique passwords. Avoid using your name or birthday."},
            {"scam", "🚫 Be cautious of unexpected messages asking for money or personal info."},
            {"privacy", "🔒 Review your privacy settings regularly and limit data sharing."}
        };

        static List<string> phishingTips = new List<string>()
        {
            "🎣 Be cautious of emails with urgent requests or suspicious links.",
            "🎣 Never provide personal information via email unless you're sure of the source.",
            "🎣 Hover over links to check if they're legitimate before clicking."
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ShowAsciiArt();
            GreetUser();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{userName}: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.ToLower().Trim();
                conversationHistory.Add(input);

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("🤖 Can you please type something?");
                    Speak("Can you please type something?");
                    continue;
                }

                if (input == "exit" || input == "quit")
                {
                    Console.WriteLine("👋 Stay safe out there, and thanks for chatting!");
                    Speak("Stay safe out there, and thanks for chatting!");
                    break;
                }

                if (DetectSentiment(input)) continue;

                if (input.Contains("interested in"))
                {
                    int index = input.IndexOf("interested in");
                    favoriteTopic = input.Substring(index + 13).Trim();
                    string response = $"🧠 Got it! I'll remember you're interested in {favoriteTopic}.";
                    Console.WriteLine(response);
                    Speak(response);
                    continue;
                }

                bool keywordMatched = HandleKeywordRecognition(input);

                if (input.Contains("phishing"))
                {
                    ProvidePhishingTip();
                    keywordMatched = true;
                }

                if (input.Contains("more") || input.Contains("details"))
                {
                    ContinuePreviousTopic();
                    continue;
                }

                if (!keywordMatched)
                {
                    Console.WriteLine("🤔 I'm not sure I understand. Could you rephrase that?");
                    Speak("I'm not sure I understand. Could you rephrase that?");
                }
            }
        }

        static void Speak(string text)
        {
            synthesizer.SpeakAsync(text);
        }

        static void ShowAsciiArt()
        {
            Console.WriteLine(@"
   ____                 _                            _           
  / ___|___  _ __   ___| |__   __ _ _ __   __ _  ___| | ___  ___ 
 | |   / _ \| '_ \ / __| '_ \ / _` | '_ \ / _` |/ _ \ |/ _ \/ __|
 | |__| (_) | | | | (__| | | | (_| | | | | (_| |  __/ |  __/\__ \
  \____\___/|_| |_|\___|_| |_|\__,_|_| |_|\__, |\___|_|\___||___/
                                           |___/                  
");
        }

        static void GreetUser()
        {
            string greeting = "Hello! I'm your Cybersecurity Awareness Chatbot. What's your name?";
            Console.Write("🤖 " + greeting + " ");
            Speak(greeting);

            userName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userName)) userName = "User";

            string welcome = $"Welcome, {userName}! I can help you with cybersecurity tips and advice.";
            Console.WriteLine($"👋 {welcome}");
            Console.WriteLine("💡 You can ask me about topics like password safety, scams, privacy, or phishing.");
            Console.WriteLine("🔚 Type 'exit' anytime to end our chat.e");
            Speak(welcome);
        }

        static bool HandleKeywordRecognition(string input)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    string response = keywordResponses[keyword];
                    Console.WriteLine(response);
                    Speak(response);

                    if (!string.IsNullOrEmpty(favoriteTopic) && input.Contains(favoriteTopic))
                    {
                        string personalized = $"🔁 As someone interested in {favoriteTopic}, that’s especially important.";
                        Console.WriteLine(personalized);
                        Speak(personalized);
                    }

                    return true;
                }
            }
            return false;
        }

        static void ProvidePhishingTip()
        {
            Random rand = new Random();
            int index = rand.Next(phishingTips.Count);
            string tip = phishingTips[index];
            Console.WriteLine(tip);
            Speak(tip);
        }

        static bool DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("anxious") || input.Contains("scared"))
            {
                string msg = "😟 It's completely okay to feel that way. Cyber threats are real, but you're not alone.";
                Console.WriteLine(msg);
                Speak("It's completely okay to feel that way. Cyber threats are real, but you're not alone.");
                Console.WriteLine("💡 Want a tip to help you feel more secure?");
                Speak("Want a tip to help you feel more secure?");
                return true;
            }
            else if (input.Contains("curious") || input.Contains("interested"))
            {
                string msg = "🧐 Great! I'll remember and I love your curiosity! Let's dive into a cybersecurity topic as someone interested in privacy, you might want to check out the security settings on your accounts.";
                Console.WriteLine(msg);
                Speak("Great! I'll remember and I love your curiosity! Let's dive into a cybersecurity topic as someone interested in privacy, you might want to check out the security settings on your accounts..");
                return true;
            }
            else if (input.Contains("frustrated") || input.Contains("confused"))
            {
                string msg = "😣 I'm here to help! Let me know which part is confusing so I can explain better.";
                Console.WriteLine(msg);
                Speak("I'm here to help! Let me know which part is confusing so I can explain better.");
                return true;
            }
            return false;
        }

        static void ContinuePreviousTopic()
        {
            foreach (string pastInput in conversationHistory.AsReadOnly())
            {
                foreach (var keyword in keywordResponses.Keys)
                {
                    if (pastInput.Contains(keyword))
                    {
                        string msg = $"🔁 Continuing our discussion on {keyword}...";
                        Console.WriteLine(msg);
                        Console.WriteLine(keywordResponses[keyword]);
                        Speak("Continuing our discussion on " + keyword + ". " + keywordResponses[keyword]);
                        return;
                    }
                }
            }

            string fallback = "🧠 I'm not sure which topic to continue. Can you tell me what you're asking about?";
            Console.WriteLine(fallback);
            Speak("I'm not sure which topic to continue. Can you tell me what you're asking about?");
        }
    }
}
