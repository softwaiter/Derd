# netcoreORM
.NetCore轻量级ORM框架

#### 模型定义

模型定义采用xml格式，每个文件支持定义一个模型，模型中可定义任意多个属性。

```
<?xml version="1.0" encoding="utf-8" ?>
<model name="User" table="t_user">
	<property name="Name" field="f_name" length="32" notNull="true" />
</model>
```

##### model属性：

name

​			指定模型名称，对当前模型的所有操作都将以该名称为标记。

table

​			对应模型的物理表名称，如果未设置，将采用name属性值；可选。

##### property属性：

name

​			属性的名称，针对该属性的所有操作都将以该名称为标记。

field

​			对应模型的物理表字段的名称，如果未设置，将采用name属性值；可选。

type

​			属性类型，支持String、Boolean、Int16、UInt16、Int32、UInt32、Int64、UInt64、Single、Double、Decimal、DateTime等，如果未设置，经默认为String；可选。

fieldType

​			属性对应物理表字段的类型，

length

​			整数型，

precision

​			整数型，

primary

​			布尔型，指定当前属性是否为对应物理表的主键，默认为False；可选。

autoIncrement

​			布尔型，指定当前属性是否为自增类型，属性type类型必须为整型，默认为False；可选。

notNull

​			布尔型，属性值是否不允许为空，默认为False；可选。

unsigned

​			布尔型，指定属性是否为无符号数，属性type类型必须为数值型，默认为False；可选。

uniqueGroup

​			字符串，

desc

​			字符串，



#### 模型方法

createTable()

removeTable()

TruncateTable()

ToString()

#### OrmUtils

OrmUtils是所有功能的入口类。

##### 方法：

public Model Model(string modelName);
