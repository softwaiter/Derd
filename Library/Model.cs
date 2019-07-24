using System.Collections.Concurrent;

namespace CodeM.Common.Orm
{
    public class Model
    {
        private ConcurrentDictionary<string, Property> mProperties = new ConcurrentDictionary<string, Property>();

//User.mapping.xml
//<mappings>
//    <mapping name = "User" table="">
//        <relation></relation>
//    </mapping>
//</mappings>
//.connection.xml
//<connection>
//    <dialect>mysql</dialect>
//    <host>127.0.0.1</host>
//    <port>3306</port>
//    <user>root</user>
//    <pass>root</pass>
//    <database>ddn</database>
//    <pool max = "" min="" idle="" />
//</connection>

        public string Name { get; set; }

        public string Table { get; set; }

        public void AddProperty(Property property)
        {
        }

    }
}
