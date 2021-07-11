using System.Collections.Generic;

namespace Netension.Request.Sample.Contexts
{
    public class SampleContext
    {
        private readonly List<string> _data = new();

        public IEnumerable<string> Get() => _data;
        public string Get(string value) => _data[_data.IndexOf(value)];
        public void Add(string value) => _data.Add(value);
        public void Update(string originalValue, string newValue) => _data[_data.IndexOf(originalValue)] = newValue;
        public void Delete(string value) => _data.Remove(value);
    }
}
