using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moserware.Skills;

namespace SkillKeeper
{
    static class EloCalculator
    {
        public static Tuple<EloRating, EloRating> CalculateElo (Person p1, Person p2, int winnerIndex, Double startingK = 800)
        {
            p1.Matches += 1;
            p2.Matches += 1;
            EloRating p1r = new EloRating();
            EloRating p2r = new EloRating();
            double Ea = 1 / (1 + Math.Pow(10, ((p2.Elo - p1.Elo) / 400)));
            double Eb = 1 / (1 + Math.Pow(10, ((p1.Elo - p2.Elo) / 400)));
            p1.K = startingK / p1.Matches;
            p2.K = startingK / p2.Matches;
            if (winnerIndex == 1)
            {
                p1.Wins += 1;
                p2.Losses += 1;
                p1r.Elo = p1.Elo + p1.K * (1 - Ea);
                p2r.Elo = p2.Elo + p2.K * (0 - Eb);
            }
            else if (winnerIndex == 2)
            {
                p2.Wins += 1;
                p1.Losses += 1;
                p1r.Elo = p1.Elo + p1.K * (0 - Ea);
                p2r.Elo = p2.Elo + p2.K * (1 - Eb);
            }
            Tuple<EloRating, EloRating> retRating = new Tuple<EloRating, EloRating>(p1r, p2r);
            return retRating;
        } 

    }
}
