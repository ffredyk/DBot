using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Discord;
using Discord.API;
using Discord.Audio;
using Discord.Commands;
using Discord.Net;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using _2CaptchaAPI;

using RestSharp;
using System.Diagnostics;
using System.Threading;

using Websocket.Client;
using ActiveUp.Net.Mail;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.RegularExpressions;
using Message = ActiveUp.Net.Mail.Message;

namespace DBot
{

    //https://discord.gg/jFVTbzT5WA - TEST
    //https://discord.gg/mAHnRRCwxz - COOLMAN
    class Program
    {
        //public static DiscordSocketClient Client;

        private const TokenType UserTokenType = (TokenType)3;

        //1.32.59.217:47045
        //212.98.243.155:3128
        //206.189.90.21:8080  - propalena
        public static string Proxy = "212.98.243.155:3128";
        //public static string Proxy = "34.67.105.73:3128"; //pomala
        public static string Token = "ODIxNjcxMjAwMzI0NDUyMzYy.YFHG8Q.MFg-IMhOMKnUTbi22SleFaaNpFg";
        public static string Login = "DB0t19dwt3hs";

        public static WebProxy ProxyObj = new WebProxy(Proxy);
        public static Random random = new Random();
        public static _2Captcha captcha = new _2Captcha("5093eb3ba7608e487fd05f6abbeb2f21");
        public static RestClient client = new RestClient("https://discordapp.com/api/v6/");

        //TEST
        public static string InviteID = "jFVTbzT5WA";
        public static string ChannelID = "821561138616598560"; //
        public static string GuildID = "821561138155618305";

        public static bool UseDefaultProxy = false;


        static void Main(string[] args)
        {
            ProxyLogic.UpdateList().GetAwaiter().GetResult();
            DoNewShit().GetAwaiter().GetResult();
            //Client.LoginAsync(TokenType.User, )
        }

        public static async Task PrepareProxy()
        {
            //if (UseDefaultProxy) return;
            while (true)
            {
                //PROXY PICK
                string[] lines = File.ReadAllLines("proxies.txt");
                if(!UseDefaultProxy) Proxy = lines[random.Next(0, lines.Length - 1)];
                WebProxy proxy = new WebProxy(Proxy);
                Console.WriteLine($"- Using Proxy: {Proxy}");


                //REST TEST
                Stopwatch watch = new Stopwatch();
                watch.Start();

                Console.WriteLine($"- PROXY TEST..");
                var clientTest = new RestClient("https://reqres.in/api/");
                var request = new RestRequest("users");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                request.Timeout = 15000;
                clientTest.Proxy = proxy;

                var response = await clientTest.ExecutePostAsync(request);
                var content = response.Content;

                watch.Stop();
                Console.WriteLine($"- Elapsed: {watch.Elapsed.TotalSeconds}s");

                if (content.Length > 0) Console.WriteLine($"- Proxy usable!");
                if (content.Length == 0) continue;
                if (watch.ElapsedMilliseconds > (15 * 1000)) continue;

                ProxyObj = new WebProxy(Proxy);
                client.Proxy = ProxyObj;
                break;
            }
        }

        static async Task DoTempShit()
        {
            //CAPTCHA INIT
            var captcha = new _2Captcha("5093eb3ba7608e487fd05f6abbeb2f21");
            var balance = await captcha.GetBalance();
            Console.WriteLine($"2Captcha Balance: {balance.Response}$");

            //PROXY PREP
            WebProxy proxy = new WebProxy(Proxy);
            Console.WriteLine($"Using Proxy: {Proxy}");

            //REST TEST
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine($"REST request test..");
            var client = new RestClient("https://reqres.in/api/");
            var request = new RestRequest("users");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
            request.Timeout = 15000;
            client.Proxy = proxy;
            //var response = await client.ExecutePostAsync(request);
            var content = "s"; //response.Content;
            watch.Stop();
            Console.WriteLine($"Elapsed: {watch.Elapsed.TotalSeconds}s");
            if (content.Length > 0) Console.WriteLine($"Proxy usable - returned content ({content})");

            //Console.WriteLine("Press ENTER to procceed");
            //Console.ReadLine();

            ///////

            Random rand = new Random();

            try
            {
                Console.WriteLine("Using RestSharp");
                client = new RestClient("https://discordapp.com/api/v6/");
                client.Proxy = proxy;
                /////////////////////////////////////////////

                Imap4Client em = new Imap4Client();
                Console.WriteLine(em.ConnectSsl("wes1-imap2.wedos.net", 993));
                Console.WriteLine(em.Login("bordel@pawno.cz", "-Poqwlkas1")); //custom

                var mails = em.SelectMailbox("INBOX");
                var msgs = mails.SearchParse("ALL").Cast<Message>();
                Console.WriteLine($"Loaded emails {msgs.Count()}");


                bool didIt = false;

                //MAIL BROWSE
                foreach (Message mail in msgs)
                {
                    foreach(var to in mail.To) Console.WriteLine($"{to.Email} vs {Login.ToLower()}");

                    if (mail.To[0].Email.CompareTo($"{Login.ToLower()}@coolman.fun") == 0)
                    {
                        Console.WriteLine("Found email");
                        var html = mail.BodyHtml.Text;
                        Regex rg = new Regex("https:\\/\\/click\\.discord\\.com\\/ls\\/click\\?upn=(.*)\" *style.*\n.*Verify");
                        var link = rg.Match(html);
                        if (link.Success)
                        {
                            foreach (var grp in link.Groups) Console.WriteLine($"Group:  <<<{grp}>>>");

                            //VERIFY LINK RESOLVE
                            Console.WriteLine($"Original: {link.Groups[1]}{Environment.NewLine}");
                            var resolved = GetFinalRedirect("https://click.discord.com/ls/click?upn=" + link.Groups[1]);
                            Console.WriteLine($"Resolved: {resolved}{Environment.NewLine}");
                            resolved = resolved.Substring(33);
                            Console.WriteLine($"Token: {resolved}{Environment.NewLine}");

                            Console.WriteLine("Press ENTER to procceed");
                            Console.ReadLine();

                            //CAPTCHA
                            Console.WriteLine($"Solving Captcha..");
                            var resCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                            Console.WriteLine($"Success: {resCaptcha.Success}");

                            //VERIFY
                            request = new RestRequest("auth/verify");
                            request.RequestFormat = DataFormat.Json;
                            request.AddJsonBody(new
                            {
                                token = resolved,
                                captcha_key = resCaptcha.Response
                            });
                            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                            var response = await client.ExecutePostAsync(request);
                            Console.WriteLine($"Login: {response.Content}");
                            Console.ReadLine();
                            ///////////////////
                            didIt = true;
                            break;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static async Task DoNewShit()
        {
            string registerCaptchaToken = "";

            //MAIN BLOCK
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC INITIALIZATION");
                    Console.WriteLine();

                    //PROXY PREPARE
                    await PrepareProxy();

                    //CLIENT NAME
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    string randstring = new string(Enumerable.Range(1, 10).Select(_ => chars[random.Next(chars.Length)]).ToArray()).ToLower();
                    Console.WriteLine($"Random user account: DB{randstring}");
                    Login = "DB"+randstring;

                    //CAPTCHA BALANCE
                    var balance = await captcha.GetBalance();
                    Console.WriteLine($"- 2Captcha Balance: {balance.Response}$");

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (MAIN BLOCK): {e.Message}");
                    Console.WriteLine("Press ENTER to restart the block");
                    Console.ReadLine();
                    continue;
                }
            }

            //REGISTER BLOCK
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC REGISTER");
                    Console.WriteLine();

                    //CAPTCHA PREPARE - REGISTER
                    Console.WriteLine($"- Solving register CAPTCHA..");
                    //var captcha = await captcha.SolveHCaptcha("f5561ba9-8f1e-40ca-9b5b-a0b3f719ef34", "https://discordapp.com/api/v6/auth/register", proxypick, _2CaptchaAPI.Enums.ProxyType.Https);
                    var regCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/api/v6/auth/register", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                    Console.WriteLine($"- Success: {regCaptcha.Success}");
                    registerCaptchaToken = regCaptcha.Response;
                    if (!regCaptcha.Success)
                    {
                        Console.WriteLine($"- Captcha solver failed: {regCaptcha.Response}");
                        Console.WriteLine($"- Restarting block");
                        Console.WriteLine("Press ENTER to restart the block");
                        Console.ReadLine();
                        continue;
                    }

                    //REGISTER REQUEST
                    Console.WriteLine("- Sending register request to Discord");
                    var request = new RestRequest("auth/register");
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(new
                    {
                        email = $"{Login}@coolman.fun",
                        username = $"Coolman",
                        password = ";DBAccess1",
                        invite = "",
                        consent = true,
                        gift_code_sku_id = "",
                        captcha_key = regCaptcha.Response
                    });
                    request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                    var response = await client.ExecutePostAsync(request);
                    Console.WriteLine($"- Register: {response.Content}");

                    if (response.Content.Length == 0)
                    {
                        Console.WriteLine("- Response invalid, press ENTER to repeat process");
                        Console.ReadLine();
                        continue;
                    }
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (REGISTER BLOCK): {e.Message}");
                    Console.WriteLine("Press ENTER to restart the block");
                    Console.ReadLine();
                    continue;
                }
            }

            //VERIFY BLOCK
            await VerifyAccount();

            //LOGIN BLOCK
            bool doLoginCaptcha = false;
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC LOGIN");
                    Console.WriteLine();

                    string captchaCode = "null";
                    if(doLoginCaptcha)
                    {
                        //CAPTCHA
                        Console.WriteLine($"- Solving Login Captcha..");
                        var logCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                        Console.WriteLine($"- Success: {logCaptcha.Success}");
                        if (!logCaptcha.Success)
                        {
                            Console.WriteLine($"- Captcha solver failed: {logCaptcha.Response}");
                            Console.WriteLine($"- Restarting block");
                            Console.WriteLine("Press ENTER to restart the block");
                            Console.ReadLine();
                            continue;
                        }
                        captchaCode = logCaptcha.Response;
                    }

                    //LOGIN
                    var request = new RestRequest("auth/login");
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(new
                    {
                        email = $"{Login}@coolman.fun",
                        password = ";DBAccess1",
                        undelete = "false",
                        captcha_key = captchaCode,
                        login_source = "null"
                    });
                    request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                    var response = await client.ExecutePostAsync(request);
                    dynamic data = JObject.Parse(response.Content);
                    if (data.ContainsKey("token"))
                    {
                        Token = data.token;
                        Console.WriteLine($"Login: {response.Content}");
                        break;
                    }
                    else
                    {
                        if (data.ContainsKey("retry_after"))
                        {
                            Console.WriteLine($"- Rate limited.. Waiting {data.retry_after}ms..");
                            await Task.Delay(int.Parse(data.retry_after));
                            Console.WriteLine("- Restarting block..");
                            continue;
                        }

                        Console.WriteLine("- Login failed, maybe CAPTCHA?");
                        Console.WriteLine(response.Content);
                        Console.WriteLine("- Restarting block");
                        doLoginCaptcha = true;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (LOGIN BLOCK): {e.Message}");
                    Console.WriteLine("Press ENTER to restart the block");
                    Console.ReadLine();
                    continue;
                }
            }

            //INVITE & GATEWAY BLOCK
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC INVITE & GATEWAY");
                    Console.WriteLine();

                    //INVITE
                    Console.WriteLine($"- Accepting invite to server..");
                    var request = new RestRequest($"invite/{InviteID}");
                    request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                    request.AddHeader("Authorization", Token);
                    var response = await client.ExecutePostAsync(request);
                    Console.WriteLine($"- Invite: {response.Content}");

                    dynamic data = JObject.Parse(response.Content);
                    if (data.ContainsKey("code") && data.code == 40002)
                    {
                        Console.WriteLine($"- Verification fucked up, repeating..");
                        await VerifyAccount();
                        Console.WriteLine($"- Restarting block..");
                        continue;
                    }
                    if (data.ContainsKey("retry_after"))
                    {
                        Console.WriteLine($"- Rate limited.. Waiting {data.retry_after}ms..");
                        await Task.Delay(int.Parse(data.retry_after));
                        Console.WriteLine("- Restarting block..");
                        continue;
                    }
                    ///////////////////

                    //GATEWAY
                    Console.WriteLine("- Connecting to Discord Gateway..");
                    var factory = new Func<ClientWebSocket>(() => new ClientWebSocket
                    {
                        Options =
                            {
                                Proxy = ProxyObj
                            }
                    });
                    var exitEvent = new ManualResetEvent(false);
                    using (var wsc = new WebsocketClient(new Uri("wss://gateway.discord.gg"), factory))
                    {
                        wsc.ReconnectTimeout = TimeSpan.FromSeconds(30);
                        //wsc.ReconnectionHappened.Subscribe(info =>
                        //Log.Information($"Reconnection happened, type: {info.Type}"));

                        wsc.MessageReceived.Subscribe(msg =>
                        {
                            Console.WriteLine($"Gateway incoming: {msg}");
                            exitEvent.Set();
                        });
                        wsc.Start();

                        //Task.Run(() => wsc.Send("{ message }"));

                        exitEvent.WaitOne();
                        //Console.ReadLine();
                        wsc.Send("{\"op\":2,\"d\":{\"token\":\"" + Token + "\",\"intents\":7,\"properties\":{\"$os\":\"linux\",\"$browser\":\"chrome\",\"$device\":\"build\"},\"compress\":true,\"large_threshold\":250,\"guild_subscriptions\":false,\"shard\":[0,1],\"presence\":{\"activities\":[{\"name\":\"What's up\",\"type\":0}],\"status\":\"dnd\",\"since\":91879201,\"afk\":false}}}");
                        //Console.ReadLine();
                        exitEvent.WaitOne();
                    }

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (INVITE & GATEWAY BLOCK): {e.Message}");
                    Console.WriteLine("Press ENTER to restart the block");
                    Console.ReadLine();
                    continue;
                }
            }

            //SPAM BLOCK
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC SPAM");
                    Console.WriteLine();

                    Console.WriteLine("- Bot setup successful - SPAM READY");
                    Console.WriteLine("Press ENTER to start");
                    Console.ReadLine();
                    while (true)
                    {
                        var chat = File.ReadAllLines("chat.txt");
                        var request = new RestRequest($"channels/{ChannelID}/messages");
                        //request.RequestFormat = DataFormat.Json;
                        request.AddQueryParameter("Authorization", Token);
                        request.AddParameter("Authorization", Token);
                        request.AddJsonBody(new
                        {
                            authorization = Token,
                            content = chat[random.Next(chat.Length)],
                            nonce = ((DateTime.Now.Ticks - 1420070400000) * 4194304),
                            tts = "false"
                        });
                        request.AddHeader("Authorization", Token);
                        var response = await client.ExecutePostAsync(request);
                        Console.WriteLine($"Message: {response.Content}");
                        dynamic data = JObject.Parse(response.Content);

                        if (data.ContainsKey("code") && data.code == 40002) await VerifyAccount();

                        if (data.ContainsKey("retry_after")) await Task.Delay(int.Parse(data.retry_after));
                        else await Task.Delay(random.Next(1000, 4000));
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (SPAM BLOCK): {e.Message}");
                    Console.WriteLine("Press ENTER to restart the block");
                    Console.ReadLine();
                    continue;
                }
            }
        }

        static bool accountVerified = false;
        static async Task VerifyAccount()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"--------------------------------------------------------------------");
                    Console.WriteLine($"LOGIC VERIFY");
                    Console.WriteLine();

                    //SEND NEW CODE
                    if (accountVerified)
                    {
                        Console.WriteLine($"- Requesting new verify email");
                        var request = new RestRequest($"auth/verify/resend");
                        request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                        request.AddHeader("Authorization", Token);
                        var response = await client.ExecutePostAsync(request);
                        Console.WriteLine($"- Resend: {response.Content}");

                        dynamic data = JObject.Parse(response.Content);
                        if (data.ContainsKey("retry_after"))
                        {
                            Console.WriteLine($"- Rate limited.. Waiting {data.retry_after}ms..");
                            await Task.Delay(int.Parse(data.retry_after));
                            Console.WriteLine("- Restarting block..");
                            continue;
                        }

                        if (response.Content.Length == 0)
                        {
                            Console.WriteLine("- Resend botched.. Restarting block..");
                            continue;
                        }
                        accountVerified = false;
                    }

                    Imap4Client em = new Imap4Client();
                    Console.WriteLine(em.ConnectSsl("wes1-imap2.wedos.net", 993));
                    Console.WriteLine(em.Login("bordel@pawno.cz", "-Poqwlkas1")); //custom

                    var mails = em.SelectMailbox("INBOX");
                    var msgs = mails.SearchParse("ALL").Cast<Message>();
                    Console.WriteLine($"- Loaded emails {msgs.Count()}");

                    if (msgs.Count() == 0) continue;

                    bool didIt = false;
                    //MAIL BROWSE
                    foreach (Message mail in msgs)
                    {
                        if (didIt) continue; //??
                        if (mail.To[0].Email == $"{Login.ToLower()}@coolman.fun")
                        {
                            var html = mail.BodyHtml.Text;
                            Regex rg = new Regex("https:\\/\\/click\\.discord\\.com\\/ls\\/click\\?upn=(.*)\" *style.*\n.*Verify");
                            var link = rg.Match(html);
                            if (link.Success)
                            {
                                //VERIFY LINK RESOLVE
                                Console.WriteLine($"- Original: {link.Groups[1]}{Environment.NewLine}");
                                var resolved = GetFinalRedirect("https://click.discord.com/ls/click?upn=" + link.Groups[1]);
                                Console.WriteLine($"- Resolved: {resolved}{Environment.NewLine}");
                                resolved = resolved.Substring(33);
                                Console.WriteLine($"- Token: {resolved}{Environment.NewLine}");

                               // mails.DeleteMessage(mail.IndexOnServer + 1, true);

                                //CAPTCHA
                                Console.WriteLine($"- Solving Verification Captcha..");
                                var verCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                                Console.WriteLine($"- Success: {verCaptcha.Success}");
                                if (!verCaptcha.Success)
                                {
                                    Console.WriteLine($"- Captcha solver failed: {verCaptcha.Response}");
                                    Console.WriteLine($"- Restarting block");
                                    Console.WriteLine("Press ENTER to restart the block");
                                    Console.ReadLine();
                                    continue;
                                }

                                //VERIFY
                                var request = new RestRequest("auth/verify");
                                request.RequestFormat = DataFormat.Json;
                                request.AddJsonBody(new
                                {
                                    token = resolved,
                                    captcha_key = verCaptcha.Response
                                });
                                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                                var response = await client.ExecutePostAsync(request);
                                Console.WriteLine($"- Verify: {response.Content}");
                                dynamic data = JObject.Parse(response.Content);
                                Token = data.token;
                                accountVerified = true;
                                ///////////////////
                                didIt = true;
                                break;
                            }
                        }
                    }
                    if (didIt) break;

                    Console.WriteLine("No valid e-mail.. Delay 5s");
                    await Task.Delay(5000);
                    continue;

                    Console.WriteLine("- Manual confirm.. Press ENTER when done");
                    Console.ReadLine();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"- ERROR (VERIFY BLOCK): {e.Message}");

                    Console.WriteLine("- Manual confirm.. Press ENTER when done");
                    Console.ReadLine();
                    break;
                }
            }
        }

        static async Task DoShit()
        {
            Random random = new Random();

            while (true)
            {
                //CAPTCHA INIT
                var captcha = new _2Captcha("5093eb3ba7608e487fd05f6abbeb2f21");
                var balance = await captcha.GetBalance();
                Console.WriteLine($"2Captcha Balance: {balance.Response}$");

                //PROXY PREP
                string[] lines = File.ReadAllLines("proxies.txt");
                Proxy = lines[random.Next(0, lines.Length - 1)];
                WebProxy proxy = new WebProxy(Proxy);
                Console.WriteLine($"Using Proxy: {Proxy}");

                //REST TEST
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Console.WriteLine($"REST request test..");
                var client = new RestClient("https://reqres.in/api/");
                var request = new RestRequest("users");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                request.Timeout = 15000;
                client.Proxy = proxy;
                var response = await client.ExecutePostAsync(request);
                var content = response.Content;
                watch.Stop();
                Console.WriteLine($"Elapsed: {watch.Elapsed.TotalSeconds}s");
                if (content.Length > 0) Console.WriteLine($"Proxy usable - returned content ({content})");
                if (content.Length == 0) continue;
                if (watch.ElapsedMilliseconds > (15 * 1000)) continue;

                //CAPTCHA SOLVE
                Console.WriteLine($"Solving Captcha..");
                //var captcha = await captcha.SolveHCaptcha("f5561ba9-8f1e-40ca-9b5b-a0b3f719ef34", "https://discordapp.com/api/v6/auth/register", proxypick, _2CaptchaAPI.Enums.ProxyType.Https);
                var resCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/api/v6/auth/register", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                Console.WriteLine($"Result: {resCaptcha.Response}");
                Console.WriteLine($"Success: {resCaptcha.Success}");

                //CLIENT NAME
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string randstring = new string(Enumerable.Range(1, 10).Select(_ => chars[random.Next(chars.Length)]).ToArray()).ToLower();
                Console.WriteLine($"Random user account: DB{randstring}");
                Login = randstring;

                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("REGISTER LOGIC START");
                Console.WriteLine();

                while (true)
                {
                    //REGISTER REQUEST
                    Console.WriteLine("Discord register request..");
                    client = new RestClient("https://discordapp.com/api/v6/");
                    request = new RestRequest("auth/register");
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(new
                    {
                        email = $"DB{randstring}@coolman.fun",
                        username = $"Coolman",
                        password = ";DBAccess1",
                        invite = "",
                        consent = true,
                        gift_code_sku_id = "",
                        captcha_key = resCaptcha.Response
                    });
                    request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                    //request.AddHeader("content-type", "application/json");
                    //request.AddHeader("Connection", "keep-alive");
                    client.Proxy = proxy;
                    response = await client.ExecutePostAsync(request);
                    Console.WriteLine($"Register: {response.Content}");

                    if (response.Content.Length == 0)
                    {
                        Console.WriteLine("Response invalid, press ENTER to repeat process");
                        Console.ReadLine();
                        continue;
                    }

                    //RESPONSE PARSE
                    dynamic json = JObject.Parse(response.Content);
                    if (json.ContainsKey("token"))
                    {
                        Token = json.token;
                        Console.WriteLine($"Token: {Token}");

                        bool didIt = false;
                        while (true)
                        {
                            Imap4Client em = new Imap4Client();
                            Console.WriteLine(em.ConnectSsl("wes1-imap2.wedos.net", 993));
                            Console.WriteLine(em.Login("bordel@pawno.cz", "-Poqwlkas1")); //custom

                            var mails = em.SelectMailbox("INBOX");
                            var msgs = mails.SearchParse("ALL").Cast<Message>();
                            Console.WriteLine($"Loaded emails {msgs.Count()}");

                            if (msgs.Count() == 0) continue;

                            //MAIL BROWSE
                            foreach (Message mail in msgs)
                            {
                                if (mail.To[0].Email == $"db{randstring}@coolman.fun")
                                {
                                    var html = mail.BodyHtml.Text;
                                    Regex rg = new Regex("https:\\/\\/click\\.discord\\.com\\/ls\\/click\\?upn=(.*)\" *style.*\n.*Verify");
                                    var link = rg.Match(html);
                                    if (link.Success)
                                    {
                                        //VERIFY LINK RESOLVE
                                        Console.WriteLine($"Original: {link.Groups[1]}{Environment.NewLine}");
                                        var resolved = GetFinalRedirect("https://click.discord.com/ls/click?upn=" + link.Groups[1]);
                                        Console.WriteLine($"Resolved: {resolved}{Environment.NewLine}");
                                        resolved = resolved.Substring(33);
                                        Console.WriteLine($"Token: {resolved}{Environment.NewLine}");

                                        //CAPTCHA
                                        Console.WriteLine($"Solving Captcha..");
                                        resCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/", Proxy, _2CaptchaAPI.Enums.ProxyType.Https);
                                        Console.WriteLine($"Success: {resCaptcha.Success}");

                                        //VERIFY
                                        request = new RestRequest("auth/verify");
                                        request.RequestFormat = DataFormat.Json;
                                        request.AddJsonBody(new
                                        {
                                            token = resolved,
                                            captcha_key = resCaptcha.Response
                                        });
                                        request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                                        response = await client.ExecutePostAsync(request);
                                        Console.WriteLine($"Login: {response.Content}");
                                        dynamic data = JObject.Parse(response.Content);
                                        Token = data.token;
                                        ///////////////////
                                        didIt = true;
                                        break;
                                    }
                                }
                            }
                            if (didIt) break;
                            Console.WriteLine("No e-mail.. Delay 5s");
                            await Task.Delay(5000);
                            continue;
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Response invalid, press ENTER to repeat process");
                        Console.ReadLine();
                        continue;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("LOGIN & SPAM LOGIC START");
                Console.WriteLine();

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Using RestSharp");
                        client = new RestClient("https://discordapp.com/api/v6/");
                        client.Proxy = proxy;
                        /////////////////////////////////////////////

                        //LOGIN
                        request = new RestRequest("auth/login");
                        request.RequestFormat = DataFormat.Json;
                        request.AddJsonBody(new
                        {
                            email = $"{Login}@coolman.fun",
                            password = ";DBAccess1",
                            undelete = "false",
                            captcha_key = "null",
                            login_source = "null"
                        });
                        request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                        response = await client.ExecutePostAsync(request);
                        dynamic data = JObject.Parse(response.Content);
                        Token = data.token;
                        Console.WriteLine($"Login: {response.Content}");
                        //Console.ReadLine();
                        ///////////////////

                        //INVITE
                        request = new RestRequest($"invite/{InviteID}");
                        request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
                        request.AddHeader("Authorization", Token);
                        response = await client.ExecutePostAsync(request);
                        Console.WriteLine($"Invite: {response.Content}");
                        //Console.ReadLine();
                        ///////////////////

                        //GATEWAY
                        Console.WriteLine("Connecting to Discord Gateway");
                        var factory = new Func<ClientWebSocket>(() => new ClientWebSocket
                        {
                            Options =
                            {
                                Proxy = proxy
                            }
                        });
                        var exitEvent = new ManualResetEvent(false);
                        using (var wsc = new WebsocketClient(new Uri("wss://gateway.discord.gg"), factory))
                        {
                            wsc.ReconnectTimeout = TimeSpan.FromSeconds(30);
                            //wsc.ReconnectionHappened.Subscribe(info =>
                            //Log.Information($"Reconnection happened, type: {info.Type}"));

                            wsc.MessageReceived.Subscribe(msg =>
                            {
                                Console.WriteLine($"Gateway incoming: {msg}");
                                exitEvent.Set();
                            });
                            wsc.Start();

                            //Task.Run(() => wsc.Send("{ message }"));

                            exitEvent.WaitOne();
                            //Console.ReadLine();
                            wsc.Send("{\"op\":2,\"d\":{\"token\":\"" + Token + "\",\"intents\":7,\"properties\":{\"$os\":\"linux\",\"$browser\":\"chrome\",\"$device\":\"build\"},\"compress\":true,\"large_threshold\":250,\"guild_subscriptions\":false,\"shard\":[0,1],\"presence\":{\"activities\":[{\"name\":\"What's up\",\"type\":0}],\"status\":\"dnd\",\"since\":91879201,\"afk\":false}}}");
                            //Console.ReadLine();
                            exitEvent.WaitOne();
                        }

                        //SPAM LOOP (unbreakable)
                        Console.WriteLine("Setup successful - SPAM READY");
                        Console.WriteLine("Press ENTER to start");
                        Console.ReadLine();
                        while (true)
                        {
                            var chat = File.ReadAllLines("chat.txt");
                            request = new RestRequest($"channels/{ChannelID}/messages");
                            //request.RequestFormat = DataFormat.Json;
                            request.AddQueryParameter("Authorization", Token);
                            request.AddParameter("Authorization", Token);
                            request.AddJsonBody(new
                            {
                                authorization = Token,
                                content = chat[random.Next(chat.Length)],
                                nonce = ((DateTime.Now.Ticks - 1420070400000) * 4194304),
                                tts = "false"
                            });
                            request.AddHeader("Authorization", Token);
                            response = await client.ExecutePostAsync(request);
                            Console.WriteLine($"Message: {response.Content}");
                            data = JObject.Parse(response.Content);

                            if(data.ContainsKey("retry_after")) await Task.Delay(int.Parse(data.retry_after));
                            else await Task.Delay(random.Next(1000, 4000));
                        };
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error S2: {e.Message}");
                        Console.ReadLine();
                        continue; //reset logic
                    }
                }

                Console.WriteLine("End of loop");
                Console.ReadLine();
                break;
            }
            Console.WriteLine("Program End");
        }

        public static string GetFinalRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}
