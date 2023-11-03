<div align="center">
<article style="display: flex; flex-direction: column; align-items: center; justify-content: center;">
    <p align="center"><img width="256" src="http://res.dayuan.tech/images/derd.png" /></p>
    <p>
        一款使用简单、功能强大的对象关系映射（O/RM）组件
    </p>
</article>
</div>


##  :beginner: 简介

Derd是一个基于.net core开发的跨平台轻量级数据库操作框架。Derd模型定义文件基于XML文件格式，模型管理基于目录自动分类；数据库类型支持Sqlite、MySql、Oracle、Sqlserver、Postgresql、达梦、人大金仓等，数据库配置文件和模型定义一样基于目录划分，并支持基于目录层级的继承能力；数据操作采用链式方式，简单易用。

#### 特点列表：

* XML格式定义模型
* JSON格式属性，像使用对象一样使用JSON
* 索引约束定义，普通索引、主键索引、组合索引...
* 关联外键定义，支持模型级深度关联
* 数据库事务支持，
* 查询方法支持链式操作，代码易写、易读、易维护
* 属性值格式约束定义&校验，模型定义同时搞定表单逻辑
* 属性值保存前自定义加工，支持动态函数处理器
* 属性值查询后自定义处理，支持动态函数处理器
* 模型增、删、改前后自定义拦截事件，轻松处理复杂逻辑
* 新建数据保存时属性是否参与保存可设置
* 更新数据保存时属性是否参与保存可设置
* 默认值设置支持自定义，支持动态函数处理器
* 查询返回dynamic动态对象，默认关联模型定义属性



## :package:安装

#### Package Manager

```shell
Install-Package Derd -Version 2.7.7
```

#### .NET CLI

```shell
dotnet add package Derd --version 2.7.7
```

#### PackageReference

```xml
<PackageReference Include="Derd" Version="2.7.7" />
```

#### Paket CLI

```shell
paket add Derd --version 2.7.7
```



## :hammer_and_wrench:使用说明

Derd基于微软的DbProviderFactory技术实现，根据实际使用的数据库类型，需要引入实现了对应数据库的DbProviderFactory相关接口的第三方包；已测试并验证通过的数据库第三方包对应关系如下：

| 数据库库类型 | 依赖包                        |
| ------------ | ----------------------------- |
| Sqlite       | System.Data.SQLite            |
| Mysql        | MySql.Data                    |
| Oracle       | Oracle.ManagedDataAccess.Core |
| SqlServer    | Microsoft.Data.SqlClient      |
| Postgres     | Npgsql                        |
| 达梦         | dmdbms.DmProvider             |
| 人大金仓     | Kdbndp                        |

使用时，可根据需要添加其中的一项或多项依赖。

*注：其他未列出的实现了微软DbProviderFactory接口的数据库理论上应全部支持，尚未实际验证；有兴趣的小伙伴可自己试验。



## :pencil:文档

- [数据库连接](https://softwaiter.github.io/Derd/#/0201)
- [模型定义](https://softwaiter.github.io/Derd/#/0202)
- [属性类型](https://softwaiter.github.io/Derd/#/0203)
- [模型用法](https://softwaiter.github.io/Derd/#/0204)
- [属性约束](https://softwaiter.github.io/Derd/#/0205)
- [拦截器](https://softwaiter.github.io/Derd/#/0206)
- [Processor](https://softwaiter.github.io/Derd/#/0207)
- [数据库事务](https://softwaiter.github.io/Derd/#/0208)



# 🎈 协议

Derd 使用 [MIT 协议](https://github.com/softwaiter/Derd/blob/master/LICENSE)
