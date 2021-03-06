﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using WdtApiLogin.Controllers;

namespace WdtApiLogin.Models
{
    public class CorrectTimeRange : ValidationAttribute
    {
        private readonly TimeSpan _maxTime;
        private readonly TimeSpan _minTime;
        private readonly TimeSpan _nextHOurOfNow;
        

        public CorrectTimeRange(int minHour, int maxHour)
        {
            _minTime = new TimeSpan(minHour, 0, 0);
            _maxTime = new TimeSpan(maxHour, 0, 0);
            _nextHOurOfNow = new TimeSpan(DateTime.Now.AddHours(1).Hour, 0, 0);
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var input = (StaffController.InputModel) validationContext.ObjectInstance;

            if (input.StartTime.TimeOfDay < _minTime || input.StartTime.TimeOfDay > _maxTime)
                return new ValidationResult(GetErrorMessageForNonToday());
            if (input.StartDate.Date == DateTime.Today.Date && 
                (input.StartTime.TimeOfDay < _nextHOurOfNow || input.StartTime.TimeOfDay > _maxTime))
                return new ValidationResult(GetErrorMessageForToday());

            return ValidationResult.Success;
        }

        private string GetErrorMessageForNonToday()
        {
            var min = DateTime.Today + _minTime;
            var max = DateTime.Today + _maxTime;
            var errorMessage =  $"Enter slot in working hours from {min:h:mm tt} and {max:h:mm tt}";
            Console.WriteLine($@"{DateTime.Now:dd-MM-yyyy HH:mm} {errorMessage}");
            return errorMessage;
        }

        private string GetErrorMessageForToday()
        {
            var min = DateTime.Today + _nextHOurOfNow;
            var max = DateTime.Today + _maxTime;
            var errorMessage = $"Enter slot in working hours from {min:h:mm tt} and {max:h:mm tt}";
            Console.WriteLine($@"{DateTime.Now:dd-MM-yyyy HH:mm} {errorMessage}");
            return errorMessage;
        }
    }
}
