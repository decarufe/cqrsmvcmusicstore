﻿ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [DEMO_CQRS], FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\DEMO_CQRS.mdf', SIZE = 2304 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

