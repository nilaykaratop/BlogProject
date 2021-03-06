using System.ComponentModel.DataAnnotations;
namespace Blog.Mapper.ViewModels.PostViewModels
{
    using System;
    public class PostIndexVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

    }
}