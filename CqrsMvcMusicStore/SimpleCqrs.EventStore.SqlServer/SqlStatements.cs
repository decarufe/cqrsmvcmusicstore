namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlStatements
    {
        internal const string GetEvents = "select eventtype, data from {0}";
        internal const string GetEventsByType = "select eventtype, data from {0} where eventtype in ('{1}')";
        internal const string GetEventsByTypeForAggregateRootID = "select eventtype, data from {0} where eventtype in ('{1}') and aggregaterootid = '{2}'";
        internal const string GetEventsByTypeForWithDates = "select eventtype, data from {0} where eventtype in ('{1}') and eventdate between @pStartDate and @pEndDate";
        internal const string InsertEvents = "insert into {0} values ('{1}', '{2}', '{3}', {4}, '{5}')";
        internal const string GetEventsByAggregateRootAndSequence = "select eventtype, data from {1} where aggregaterootid = '{2}' and sequence >= {3}";
        internal const string CreateTheEventStoreTable = @"IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'U'))
begin
create table dbo.[{0}](
   EventId int identity not null primary key,
   EventType nvarchar(255),
   AggregateRootId uniqueidentifier not null,
   EventDate datetime not null,
   Sequence int not null,
   Data nvarchar(max)
)
end";
    }
}