using System;
using System.Collections.Generic;

namespace GradeBook
{
    class Book
    {
        public Book(string name)
        {
            grades = new List<double>();
            this.name=name;
        }
        
        public void AddGrade(double x)
        {
            grades.Add(x);
        }
  
        public void ShowStats() 
        {
            var sum =0.0;
            var highGrade = double.MinValue;
            var lowGrade = double.MaxValue;
            foreach(double number in grades)
            {
                highGrade=Math.Max(number,highGrade);
                lowGrade=Math.Min(number,lowGrade);
                sum+= number;
            }
            var avg = sum/grades.Count;            
            
            Console.WriteLine($"The lowest grade is {lowGrade:N1}");
            Console.WriteLine($"The highest grade is {highGrade:N1}");
            Console.WriteLine($"The average grade is {avg:N1}");
        }

         private List<double> grades;
        private string name;


        
    }
}