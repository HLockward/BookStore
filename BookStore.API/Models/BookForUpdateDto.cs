using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class BookForUpdateDto : BookForManipulationDto
    {

        [Required(ErrorMessage = "you should fill out a description")]
        
        public override String Description { get => base.Description; set => base.Description = value; }
    }
}
