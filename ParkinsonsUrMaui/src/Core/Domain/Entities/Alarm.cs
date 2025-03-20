namespace MauiCleanTodos.Domain.Entities;

public class Alarm : BaseAuditableEntity
{
    public bool IsActive { get; set; }

    public TimeSpan Time { get; set; }

    //public bool Done
    //{
    //    get => _done;
    //    set
    //    {
    //        if (value == true && _done == false)
    //        {
    //            AddDomainEvent(new TodoItemCompletedEvent(this));
    //        }

    //        _done = value;
    //    }
    //}
}
