using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillKeeper
{
    class EloRating
    {
        private Double k;
        private Double elo;

        public Double K
        {
            get { return k; }
            set { k = value; }
        }

        public Double Elo
        {
            get { return elo; }
            set { elo = value; }
        }
    }
}
