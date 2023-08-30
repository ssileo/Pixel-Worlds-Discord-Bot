using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Casino.Logic
{
    class CoinflipLogic
    {
        public static int Roll()
        {
            Random random = new Random();
            return random.Next(1, 11); 
        }

        public static string CoinflippLogic(string userschoice, int amount)
        {
            int roll = Roll();
            int winamount = int.Parse(Math.Round((amount * 2) - (amount * 0.1), 0).ToString());
            
            if (userschoice == "heads")
            {
                if (roll > 6) 
                {
                    return $"won|heads|{winamount}";
                }
                else
                {
                    return $"lost|tails|{amount}";
                }
            }
            else if (userschoice == "tails")
            {
                if (roll < 4)
                {
                    return $"won|tails|{winamount}";
                }
                else
                {
                    return $"lost|heads|{amount}";
                }
            }
            else return string.Empty;
           
        }
    }
}
