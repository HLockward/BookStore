using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class BookForCreationDto : BookForManipulationDto
    {
        public BookForCreationDto() : base()
        {
        }
    }
    
}
