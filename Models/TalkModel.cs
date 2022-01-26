using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(100)]
        public string Abstract { get; set; }
        public int Level { get; set; }
        public SpeakerModel Speaker { get; set; }
    }
}
