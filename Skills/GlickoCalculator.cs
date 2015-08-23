using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moserware.Skills
{
    public class GlickoCalculator
    {

        public static double scalingFactor = 60.0;
        /// <summary>
        /// Calculates new ratings based on the prior ratings and team ranks.
        /// </summary>
        /// <param name="gameInfo">Parameters for the game.</param>
        /// <param name="teams">A mapping of team players and their ratings.</param>
        /// <param name="teamRanks">The ranks of the teams where 1 is first place. For a tie, repeat the number (e.g. 1, 2, 2)</param>
        /// <returns>All the players and their new ratings.</returns>
        public static IDictionary<TPlayer, Rating> CalculateNewRatings<TPlayer>(GameInfo gameInfo, IEnumerable <IDictionary<TPlayer, Rating>> teams,  params int[] teamRanks)
        {
            var rankP1 = teamRanks[0];
            var rankP2 = teamRanks[1];

            var winLoseP1 = 1.0;
            var winLoseP2 = 1.0;

            if(rankP1 < rankP2)
            {
                winLoseP2 = 0.0;
            }
            if (rankP2 < rankP1)
            {
                winLoseP1 = 0.0;
            }

            Dictionary<TPlayer, Rating> newValues = new Dictionary<TPlayer, Rating>();

            var player1 = teams.ElementAt(0);
            var player2 = teams.ElementAt(1);

            //var player1Result = calculatePlayerRating(player1.First().Key, testP1Rating, player2.First().Key, testP2Rating, 1);
            var player1Result = calculatePlayerRating(player1.First().Key, player1.First().Value, player2.First().Key, player2.First().Value, winLoseP1);
            var player2Result = calculatePlayerRating(player2.First().Key, player2.First().Value, player1.First().Key, player1.First().Value, winLoseP2);

            newValues[player1Result.Key] = player1Result.Value;
            newValues[player2Result.Key] = player2Result.Value;
            //newValues.Add(player2Result.Key, player2Result.Value);

            return newValues;
            // Just punt the work to the full implementation
            //return _Calculator.CalculateNewRatings(gameInfo, teams, teamRanks);
        }

        /// <summary>
        /// Calculates the match quality as the likelihood of all teams drawing.
        /// </summary>
        /// <typeparam name="TPlayer">The underlying type of the player.</typeparam>
        /// <param name="gameInfo">Parameters for the game.</param>
        /// <param name="teams">A mapping of team players and their ratings.</param>
        /// <returns>The match quality as a percentage (between 0.0 and 1.0).</returns>
        public static double CalculateMatchQuality<TPlayer>(GameInfo gameInfo, IEnumerable<IDictionary<TPlayer, Rating>> teams)
        {
            // Just punt the work to the full implementation
            //return _Calculator.CalculateMatchQuality(gameInfo, teams);
            return 1.0;
        }

        private static double calculateOnsetRD<TPlayer>(TPlayer player, Rating rating)
        {
            double oldRDSquared = Math.Pow(rating.StandardDeviation * scalingFactor, 2);
            double cSquared = Math.Pow(1.01, 2);
            return Math.Sqrt(oldRDSquared + cSquared);
        }

        private static double calculateg(double q, double RD)
        {
            double qSquared = Math.Pow(q, 2);
            double RDSquared = Math.Pow(RD, 2);
            double PISquared = Math.Pow(Math.PI, 2);

            double denominator = Math.Sqrt(1.0 + 3.0 * qSquared * RDSquared / PISquared);
            return 1.0 / denominator;
        }

        private static double calculateE<TPlayer>(TPlayer p1, Rating p1Rating, TPlayer p2, Rating p2Rating, double g)
        {
            double exponent = -g * ((p1Rating.Mean * scalingFactor - p2Rating.Mean * scalingFactor) / 400.0);
            double denominator = 1.0 + Math.Pow(10, exponent);
            return 1.0 / denominator;
        }

        private static double calculateq()
        {
            return 0.0058565;
            //return .003;
        }

        private static double calculatedSquared(double q, double g, double E)
        {
            double qSquared = Math.Pow(q, 2);
            double gSquared = Math.Pow(g, 2);
            double denominator = qSquared * gSquared * E * (1 - E);
            return 1.0 / denominator;
        }

        private static double calculateNewRD(double RD, double dSquared)
        {
            double RDSquared = Math.Pow(RD, 2);
            double sqrtInner = 1.0 / RDSquared + 1 / dSquared;
            double denominator = Math.Sqrt(sqrtInner);
            return 1.0 / denominator;
        }

        private static double calculateNewRating(double preRating, double q, double newRD, double g, double winLoss, double E)
        {
            double RDSquared = Math.Pow(newRD, 2);
            return preRating *scalingFactor + q * RDSquared * g * (winLoss - E);
        }

        private static KeyValuePair<TPlayer, Rating> calculatePlayerRating<TPlayer>(TPlayer player, Rating playerRating, TPlayer opponent, Rating opponentRating, double winLose)
        {
            double q = calculateq();
            double playerRD = calculateOnsetRD(player, playerRating);
            double opponentRD = calculateOnsetRD(opponent, opponentRating);
            double g = calculateg(q, opponentRD);
            double E = calculateE(player, playerRating, opponent, opponentRating, g);
            double dSquared = calculatedSquared(q, g, E);
            double newPlayerRD = calculateNewRD(playerRD, dSquared);
            double newRating = calculateNewRating(playerRating.Mean, q, newPlayerRD, g, winLose, E);
            KeyValuePair<TPlayer, Rating> retValue = new KeyValuePair<TPlayer, Rating>(player, new Rating(newRating / scalingFactor, newPlayerRD / scalingFactor));
            return retValue;
        }

    }
}
