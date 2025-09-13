using System;
using System.Collections.Generic;

namespace Common.DTO
{
    public class MatchInfo
    {
        public Guid Adress { get; set; }
        public int ParticipantCount { get; set; }
        public List<string> participants { get; set; }
    }
}

