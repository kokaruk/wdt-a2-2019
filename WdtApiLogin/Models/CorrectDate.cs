using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using WdtApiLogin.Controllers;

namespace WdtApiLogin.Models
{
    public class CorrectTimeRange : ValidationAttribute
    {
        private readonly TimeSpan _minTime;
        private readonly TimeSpan _maxTime;

        public CorrectTimeRange(int minHour, int maxHour)
        {
            _minTime = new TimeSpan(minHour, 0, 0);
            this._maxTime = new TimeSpan(maxHour, 0, 0);
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var input = (StaffController.InputModel)validationContext.ObjectInstance;

            if (input.StartTime.TimeOfDay < this._minTime || input.StartTime.TimeOfDay > this._maxTime)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            var min = DateTime.Today + this._minTime;
            var max = DateTime.Today + this._maxTime;
            return $"Enter slot in working hours from {min:h:mm tt} and {max:h:mm tt}";
        }

    }
}
