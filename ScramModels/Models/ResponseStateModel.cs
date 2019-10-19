using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class ResponseStateModel
    {
        public bool HasError { get => (int)ErrorCode >= 500; }
        public bool IsOk { get=> (int)ErrorCode < 500; }
        public EnumErrorCode ErrorCode { get; set; } = EnumErrorCode.NoError;
        public string ExceptionMessage { get; set; } = ".";
        private string _message { get; set; } = "";
        public string Message { get {
                var result = "";

                if (HasError && string.IsNullOrWhiteSpace(_message))
                {
                    var messages = MessageText.Text;

                    if (messages.ContainsKey(ErrorCode)) result = messages[ErrorCode];
                }
                else result = _message;

                return result;
            }
            set { _message = value; } }
        public object Payload { get; set; }
    }
}
