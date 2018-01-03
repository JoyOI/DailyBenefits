using System;

namespace DailyBenefits.Model
{
    public class Record
    {
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public Guid UserId { get; set; }

        public int Coins { get; set; }
    }
}
