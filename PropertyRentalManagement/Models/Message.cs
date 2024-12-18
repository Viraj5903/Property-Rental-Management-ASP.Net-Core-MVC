using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderUserId { get; set; }

    public int ReceiverUserId { get; set; }

    public string Subject { get; set; } = null!;

    public string MessageBody { get; set; } = null!;

    public DateTime MessageDateTime { get; set; }

    /// <summary>
    /// Read, Not Read
    /// </summary>
    public int StatusId { get; set; }

    public virtual User ReceiverUser { get; set; } = null!;

    public virtual User SenderUser { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
