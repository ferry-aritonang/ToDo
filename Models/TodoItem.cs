using System;

namespace ToDo.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public DateTime ExpireDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PctComplete { get; set; }
    }
}