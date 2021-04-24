using System.Text;

namespace Demo.Scripts
{
    public class InjectablePoco
    {
        private StringBuilder _stringBuilder;

        public InjectablePoco(StringBuilder stringBuilder) => _stringBuilder = stringBuilder;
    }

    public class InjectablePoco2
    {
        private StringBuilder _stringBuilder;
        private SimpleInjectablePoco _poco;

        public InjectablePoco2(StringBuilder stringBuilder, SimpleInjectablePoco poco)
        {
            _poco = poco;
            _stringBuilder = stringBuilder;
        }
    }

    public class PrivateInjectablePoco
    {
        private PrivateInjectablePoco() { }
    }

    public class SimpleInjectablePoco { }
}