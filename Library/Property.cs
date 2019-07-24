namespace CodeM.Common.Orm
{
    public class Property
    {

        public string Name { get; set; }

        public string Field { get; set; }

        public string Description { get; set; }

        public bool IsPrimaryKey { get; set; } = false;

        public bool IsNotNull { get; set; } = false;

        //Type
        //DefaultValue
        //Generator
        //Processor

        public int Length { get; set; } = 0;

        public bool JoinInsert { get; set; } = true;

        public bool JoinUpdate { get; set; } = true;

    }
}
