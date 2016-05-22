//
// Scientific Games Coding Challenge
//
// Author: Ed Rozmiarek
//
// Problem: Given a list of people with their birth and end years (all between 1900 and 2000), 
//          find the year with the most number of people alive.
//
// Assumptions:
// - "between 1900 and 2000" meant "inclusive". Therefore 1900 and 2000 are valid years.
// - A person is counted to be alive if they are alive for any part of the year.
//   Therefore a person is counted for both the year they were born and the year they died.
//
// Solution:
// I decided to keep the program simple but to try multiple possible methods to find to answer.
// This program is a standard Windows command line program. I used Visual Studio 2015 Community edition,
// but it should compile with just about any version of Visual Studio.
//
// Test Data:
// The test data is a randomly generated list of NUM_PEOPLE people. Each person has a 
// randomly generated birth year from MIN_YEAR to MIN_YEAR plus two thirds the difference 
// of MAX_YEAR and MIN_YEAR. The death year is a random year from thier birth year to MAX_YEAR.
// 
// Issues:
// - The problem states "find the year" but there may be multiple years with the same 
//   maximum population. This solition only finds the earliest year with the maximum population.
//   The solution could be easily extended to find a list of years with the maximum population.
//   However, with a large enough sample size, the chance of multiple years having the same
//   maximum population is greatly reduced.
//
// Sample output:
// Method 1: Max Population: 565457
// Method 1: Max Year: 1964
// Method 2: Max Population: 565457
// Method 2: Max Year: 1964
// Number of people: 1000000
// Method 1 time: 00:00:00.3670210
// Method 2 time: 00:00:00.0390022

using System;
using System.Collections.Generic;
using System.Text;

namespace SGCodingChallenge
{
    public class Program
    {
        /// <summary>
        /// Constants that control the program.
        /// </summary>
        const int MIN_YEAR = 1900;
        const int MAX_YEAR = 2000;
        const int NUM_PEOPLE = 1000000;

        /// <summary>
        /// Holds the list of people and their data.
        /// </summary>
        static private List<Person> People { get; set; }

        /// <summary>
        /// Array to hold the yearly populations calculated by Method1.
        /// </summary>
        static private int[] Population { get; set; }

        /// <summary>
        /// Array to hold the yearly population deltas calculate by Method2.
        /// </summary>
        static private int[] PopDelta { get; set; }

        /// <summary>
        /// Time used by Method1.
        /// </summary>
        static TimeSpan Method1Time { get; set; }

        /// <summary>
        /// Time used by Method2.
        /// </summary>
        static TimeSpan Method2Time { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            LoadPeople();
            Method1();
            Method2();
            Report();
        }

        /// <summary>
        /// Problem solution method 1.
        /// 
        /// My first thought for solving the problem. Simply determine the population
        /// for each year by keeping a counter for each year. Loop through all of the
        /// people and then loop through the years they were alive and increment the 
        /// counter for the year. Then, loop through the years to find the maximum population.
        /// Stright forward, but most likely, the slowest method.
        /// </summary>
        static void Method1()
        {
            // For timing to see which method is faster.
            DateTime startTime = DateTime.Now;

            // Create the array to hold the population for each year.
            Population = new int[MAX_YEAR - MIN_YEAR + 1];

            // Loop through all of the people
            foreach (Person person in People)
            {
                // Verify that the years are within the range we are working with.
                // Probably could skip the check given the problem description, but
                // better safe than sorry.
                if ( IsValidYear(person.BirthYear) && IsValidYear(person.DeathYear))
                {
                    // Increment the population for each year the person was alive.
                    for (int year = person.BirthYear-MIN_YEAR; year <= person.DeathYear-MIN_YEAR; year++ )
                    {
                        Population[year]++;
                    }
                }
            }

            // Find the year with the maximum population.
            // Simple search of the array to find the maximum value and which
            // which year it occured.
            int maxPopulation = 0;
            int maxYear = 0;
            for (int index = 0; index < Population.Length; index++)
            {
                if (Population[index] > maxPopulation)
                {
                    maxPopulation = Population[index];
                    maxYear = index;
                }
            }

            // Adjust the year for reporting.
            maxYear += MIN_YEAR;

            // Determine how long it took.
            Method1Time = DateTime.Now - startTime;

            // Report results.
            Console.WriteLine("Method 1: Max Population: " + maxPopulation);
            Console.WriteLine("Method 1: Max Year: " + maxYear);
        }

        /// <summary>
        /// Problem solution mentod 2.
        /// 
        /// Figuring there had to be a faster method for determining the solution,
        /// I gave the problem some additional thought and came up with this method.
        /// I would only track the population delta for each year. Then we could just 
        /// calulate the current population for the years by doing a running total based
        /// on the deltas.
        /// </summary>
        static void Method2()
        {
            // For timing to see which method is faster.
            DateTime startTime = DateTime.Now;

            // Create the array to hold the popluation deltas.
            // We need an extra value at the end to hold the delta for the year MAX_YEAR+1
            // since teh people dying in MAX_YEAR will be removed the following year.
            PopDelta = new int[MAX_YEAR - MIN_YEAR + 2];

            // Loop through the people to calculate the population deltas.
            foreach (Person person in People)
            {
                // Verify that the years are within the range we are working with.
                // Probably could skip the check given the problem description, but
                // better safe than sorry.
                if (IsValidYear(person.BirthYear) && IsValidYear(person.DeathYear))
                {
                    PopDelta[person.BirthYear - MIN_YEAR]++;
                    PopDelta[person.DeathYear - MIN_YEAR + 1]--;
                }
            }

            // Find the year with the maximum population.
            // Starting with a zero population, modify the current population
            // based on the population deltas calculated above.
            int currentPopulation = 0;  
            int maxPopulation = 0;      
            int maxYear = 0;

            // Loop through the years
            for (int index = 0; index < PopDelta.Length; index++)
            {
                // Modify the current population.
                currentPopulation += PopDelta[index];
                // Do we have a new maximum population?
                if (currentPopulation > maxPopulation)
                {
                    maxPopulation = currentPopulation;
                    maxYear = index;
                }
            }

            // Adjust the year for reporting.
            maxYear += MIN_YEAR;

            // Determine how long it took.
            Method2Time = DateTime.Now - startTime;

            // Report results.
            Console.WriteLine("Method 2: Max Population: " + maxPopulation);
            Console.WriteLine("Method 2: Max Year: " + maxYear);
        }

        /// <summary>
        /// Reports stats for the program. Mainly used for debuging and operation verification.
        /// </summary>
        static void Report()
        {
            Console.WriteLine("Number of people: " + People.Count);
            Console.WriteLine("Method 1 time: " + Method1Time);
            Console.WriteLine("Method 2 time: " + Method2Time);

            //Console.WriteLine("Population:");
            //for (int index = 0; index < m_population.Length; index++)
            //{
            //    Console.WriteLine("Year {0}: {1}, Delta: {2}", MIN_YEAR + index, m_population[index], m_popDelta[index]);
            //}
        }

        /// <summary>
        /// Helper function that determines if a year is within the valid range of years.
        /// </summary>
        /// <param name="a_year">Year to test.</param>
        /// <returns>True if the year falls within the valid range of years.</returns>
        static private bool IsValidYear(int a_year)
        {
            return a_year >= MIN_YEAR && a_year <= MAX_YEAR;
        }

        /// <summary>
        /// Generate a list of people with random birth and death years.
        /// </summary>
        static private void LoadPeople()
        {
            People = new List<Person>();
            Random random = new Random();

            int max = 2 * (MAX_YEAR - MIN_YEAR + 1) / 3;
            for (int i = 0; i < NUM_PEOPLE; i++)
            {
                int born = MIN_YEAR + random.Next(max);
                int died = born + random.Next(MAX_YEAR - born + 1);
                //Console.WriteLine("New person: {0}, {1}", born, died);
                People.Add(new Person(born, died));
            }
        }
    }

    /// <summary>
    /// Simple class to hold the data for each person.
    /// Contains just the birth and death years as properties.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Birth year
        /// </summary>
        public int BirthYear { get; set; }

        /// <summary>
        /// Death year
        /// </summary>
        public int DeathYear { get; set; }

        /// <summary>
        /// Constructor that takes both years
        /// </summary>
        /// <param name="a_birth">Year the person was born.</param>
        /// <param name="a_death">Year the person died.</param>
        public Person( int a_birth, int a_death )
        {
            BirthYear = a_birth;
            DeathYear = a_death;
        }
    }
}
