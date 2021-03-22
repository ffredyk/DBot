using _2CaptchaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBot
{
    public class FuncLogic
    {

        public static async Task<string> SolveRegisterCaptcha()
        {
            var captcha = new _2Captcha("5093eb3ba7608e487fd05f6abbeb2f21");
            var balance = await captcha.GetBalance();
            Console.WriteLine($"2Captcha Balance: {balance.Response}$");

            while (true)
            {
                var resCaptcha = await captcha.SolveReCaptchaV2("6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/api/v6/auth/register", Program.Proxy, _2CaptchaAPI.Enums.ProxyType.Https);

                if (resCaptcha.Success) return resCaptcha.Response;
                else Console.WriteLine("Captcha fucked up");
            }
        }
    }
}
