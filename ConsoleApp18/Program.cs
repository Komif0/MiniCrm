using Microsoft.VisualBasic;
using System.Net;
using System.Reflection.PortableExecutable;
using System.IO; 

namespace Program
{
    public class Program
    {
        public record Client(string name, string eMail, int id, DateTime CreatedAt);
        public record Order(int Id, int ClientId, string? Description, decimal Amount, DateOnly DueDate);

        public static void Main(string[] args)
        {
            
        }
    }
}