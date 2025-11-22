namespace MediaPlayer.Models;

internal enum MessageTemplateType : byte { Information = 0, Warning, Error }

/// <summary>
/// 
/// </summary>
internal record class MessageTemplateViewModel(string Message, MessageTemplateType Type) { }