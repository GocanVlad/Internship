using System;
using System.Collections.Generic;

namespace GradeBook
{
    public class Book
    {
        public Book(string name)
        {
            grades = new List<double>();
            Name=name;
        }
        
        public void AddGrade(double x)
        {
            grades.Add(x);
        }
  
        public Statistics GetStats() 
        {
            var result = new Statistics();

            result.Average= 0.0;
            result.High= double.MinValue;
            result.Low= double.MaxValue;
            foreach(double grade in grades)
            {
                result.High=Math.Max(grade,result.High);
                result.Low=Math.Min(grade,result.Low);
                result.Average += grade;
            }
            result.Average/=grades.Count;

            return result;
        }

         private List<double> grades;
        public string Name;


        
    }
}