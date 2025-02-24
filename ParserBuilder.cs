using CmdParser.SubParsers;

namespace CmdParser
{
    public static class ParserBuilder
    {
        public const string COMMAND = "command";
        public const string COMMAND_BODY = "command_body";
        public const string KEY_FIELD = "key_field";
        public const string VALUE_FIELD = "value_field";
        public const string KV_PARAM = "kv_param";
        public const string COMMAND_STRING = "command_str";
        public const string STRING_FIELD = "str_field";
        public const string DIC_FIELD = "dic_field";
        public const string KV_FIELD = "kv_field";
        public const string COMMON_FIELD = "common_field";
        public const string ARRAY_FIED = "array_field";

        public static ParserBase Build(string name)
        {
            switch (name)
            {
                case COMMAND:
                    return new CommandParser(name);

                case COMMAND_BODY:
                    return new CommandBodyParser(name);

                case KEY_FIELD:
                    return new KeyFieldParser(name);

                case VALUE_FIELD:
                    return new ValueFieldParser(name);

                case KV_PARAM:
                    return new KvParamParser(name);

                case COMMAND_STRING:
                    return new CommandStrParser(name);

                case STRING_FIELD:
                    return new StringFieldParser(name);

                case DIC_FIELD:
                    return new DicFieldParser(name);

                case KV_FIELD:
                    return new DicKvPairParser(name);

                case COMMON_FIELD:
                    return new CommonFieldParser(name);

                case ARRAY_FIED:
                    return new ArrayParser(name);

                default:
                    throw new UnknownParserException(name);
            }
        }
    }
}