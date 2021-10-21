using System;
using System.Collections.Generic;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = new double[]{12.7, 10.3, 6.11, 4.1};
            var grades = new List<double>() {12.7, 10.3, 6.11, 4.1}; 
            grades.Add(56.1);
            
            var sum =0.0;
            foreach(double number in grades)
                sum+= number;
            var avg = sum/grades.Count;

            
            for(var i=0;i<numbers.Length;i++)
                Console.WriteLine(numbers[i]);

            Console.WriteLine($"The sum grade is {sum}");
            Console.WriteLine($"The average grade is {avg:N1}");

            if(args.Length > 0)
                Console.WriteLine($"Hello, {args[0]}!");
            else
                Console.WriteLine("Hello!");
        }
    }
}
