// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: others.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ilruntime.Protobuf;
using pbc = global::Google.ilruntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace DragonU3DSDK.Network.API.ILProtocol {

  #region Messages
  public sealed class CHeartBeat : pb::IMessage {
    private static readonly pb::MessageParser<CHeartBeat> _parser = new pb::MessageParser<CHeartBeat>(() => new CHeartBeat());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CHeartBeat> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  public sealed class SHeartBeat : pb::IMessage {
    private static readonly pb::MessageParser<SHeartBeat> _parser = new pb::MessageParser<SHeartBeat>(() => new SHeartBeat());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SHeartBeat> Parser { get { return _parser; } }

    /// <summary>Field number for the "timestamp" field.</summary>
    public const int TimestampFieldNumber = 1;
    private ulong timestamp_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Timestamp {
      get { return timestamp_; }
      set {
        timestamp_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Timestamp != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Timestamp);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Timestamp != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Timestamp);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Timestamp = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class CGetConfig : pb::IMessage {
    private static readonly pb::MessageParser<CGetConfig> _parser = new pb::MessageParser<CGetConfig>(() => new CGetConfig());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CGetConfig> Parser { get { return _parser; } }

    /// <summary>Field number for the "route" field.</summary>
    public const int RouteFieldNumber = 1;
    private string route_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Route {
      get { return route_; }
      set {
        route_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Route.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Route);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Route.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Route);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Route = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed class SGetConfig : pb::IMessage {
    private static readonly pb::MessageParser<SGetConfig> _parser = new pb::MessageParser<SGetConfig>(() => new SGetConfig());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SGetConfig> Parser { get { return _parser; } }

    /// <summary>Field number for the "config" field.</summary>
    public const int ConfigFieldNumber = 8;
    private global::DragonU3DSDK.Network.API.ILProtocol.ClientConfig config_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::DragonU3DSDK.Network.API.ILProtocol.ClientConfig Config {
      get { return config_; }
      set {
        config_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (config_ != null) {
        output.WriteRawTag(66);
        output.WriteMessage(Config);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (config_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Config);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 66: {
            if (config_ == null) {
              config_ = new global::DragonU3DSDK.Network.API.ILProtocol.ClientConfig();
            }
            input.ReadMessage(config_);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///公告
  /// </summary>
  public sealed class Announcement : pb::IMessage {
    private static readonly pb::MessageParser<Announcement> _parser = new pb::MessageParser<Announcement>(() => new Announcement());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Announcement> Parser { get { return _parser; } }

    /// <summary>Field number for the "announcement_id" field.</summary>
    public const int AnnouncementIdFieldNumber = 1;
    private string announcementId_ = "";
    /// <summary>
    ///公告id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AnnouncementId {
      get { return announcementId_; }
      set {
        announcementId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "start_time" field.</summary>
    public const int StartTimeFieldNumber = 2;
    private ulong startTime_;
    /// <summary>
    ///公告开始时间
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong StartTime {
      get { return startTime_; }
      set {
        startTime_ = value;
      }
    }

    /// <summary>Field number for the "end_time" field.</summary>
    public const int EndTimeFieldNumber = 3;
    private ulong endTime_;
    /// <summary>
    ///公告结束时间
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong EndTime {
      get { return endTime_; }
      set {
        endTime_ = value;
      }
    }

    /// <summary>Field number for the "cd_seconds" field.</summary>
    public const int CdSecondsFieldNumber = 4;
    private ulong cdSeconds_;
    /// <summary>
    ///公告弹出间隔时间（单位：秒）
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong CdSeconds {
      get { return cdSeconds_; }
      set {
        cdSeconds_ = value;
      }
    }

    /// <summary>Field number for the "is_all" field.</summary>
    public const int IsAllFieldNumber = 5;
    private bool isAll_;
    /// <summary>
    ///是否发送全体玩家
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsAll {
      get { return isAll_; }
      set {
        isAll_ = value;
      }
    }

    /// <summary>Field number for the "player_condition" field.</summary>
    public const int PlayerConditionFieldNumber = 6;
    private string playerCondition_ = "";
    /// <summary>
    ///玩家条件筛选
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string PlayerCondition {
      get { return playerCondition_; }
      set {
        playerCondition_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "img_url" field.</summary>
    public const int ImgUrlFieldNumber = 7;
    private string imgUrl_ = "";
    /// <summary>
    ///公告图片链接
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ImgUrl {
      get { return imgUrl_; }
      set {
        imgUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "msg_type" field.</summary>
    public const int MsgTypeFieldNumber = 8;
    private global::DragonU3DSDK.Network.API.ILProtocol.Announcement.Types.MsgType msgType_ = 0;
    /// <summary>
    ///公告信息类型
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::DragonU3DSDK.Network.API.ILProtocol.Announcement.Types.MsgType MsgType {
      get { return msgType_; }
      set {
        msgType_ = value;
      }
    }

    /// <summary>Field number for the "msg_text" field.</summary>
    public const int MsgTextFieldNumber = 9;
    private string msgText_ = "";
    /// <summary>
    ///公告信息内容（若自定义类型）
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MsgText {
      get { return msgText_; }
      set {
        msgText_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "locale_msg_infos" field.</summary>
    public const int LocaleMsgInfosFieldNumber = 10;
    private static readonly pbc::MapField<string, string>.Codec _map_localeMsgInfos_codec
        = new pbc::MapField<string, string>.Codec(pb::FieldCodec.ForString(10), pb::FieldCodec.ForString(18), 82);
    private readonly pbc::MapField<string, string> localeMsgInfos_ = new pbc::MapField<string, string>();
    /// <summary>
    ///公告信息内容多语言（若自定义类型)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<string, string> LocaleMsgInfos {
      get { return localeMsgInfos_; }
    }

    /// <summary>Field number for the "manual_end" field.</summary>
    public const int ManualEndFieldNumber = 11;
    private bool manualEnd_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool ManualEnd {
      get { return manualEnd_; }
      set {
        manualEnd_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (AnnouncementId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(AnnouncementId);
      }
      if (StartTime != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(StartTime);
      }
      if (EndTime != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(EndTime);
      }
      if (CdSeconds != 0UL) {
        output.WriteRawTag(32);
        output.WriteUInt64(CdSeconds);
      }
      if (IsAll != false) {
        output.WriteRawTag(40);
        output.WriteBool(IsAll);
      }
      if (PlayerCondition.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(PlayerCondition);
      }
      if (ImgUrl.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(ImgUrl);
      }
      if (MsgType != 0) {
        output.WriteRawTag(64);
        output.WriteEnum((int) MsgType);
      }
      if (MsgText.Length != 0) {
        output.WriteRawTag(74);
        output.WriteString(MsgText);
      }
      localeMsgInfos_.WriteTo(output, _map_localeMsgInfos_codec);
      if (ManualEnd != false) {
        output.WriteRawTag(88);
        output.WriteBool(ManualEnd);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (AnnouncementId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AnnouncementId);
      }
      if (StartTime != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(StartTime);
      }
      if (EndTime != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(EndTime);
      }
      if (CdSeconds != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(CdSeconds);
      }
      if (IsAll != false) {
        size += 1 + 1;
      }
      if (PlayerCondition.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PlayerCondition);
      }
      if (ImgUrl.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ImgUrl);
      }
      if (MsgType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MsgType);
      }
      if (MsgText.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MsgText);
      }
      size += localeMsgInfos_.CalculateSize(_map_localeMsgInfos_codec);
      if (ManualEnd != false) {
        size += 1 + 1;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            AnnouncementId = input.ReadString();
            break;
          }
          case 16: {
            StartTime = input.ReadUInt64();
            break;
          }
          case 24: {
            EndTime = input.ReadUInt64();
            break;
          }
          case 32: {
            CdSeconds = input.ReadUInt64();
            break;
          }
          case 40: {
            IsAll = input.ReadBool();
            break;
          }
          case 50: {
            PlayerCondition = input.ReadString();
            break;
          }
          case 58: {
            ImgUrl = input.ReadString();
            break;
          }
          case 64: {
            msgType_ = (global::DragonU3DSDK.Network.API.ILProtocol.Announcement.Types.MsgType) input.ReadEnum();
            break;
          }
          case 74: {
            MsgText = input.ReadString();
            break;
          }
          case 82: {
            localeMsgInfos_.AddEntriesFrom(input, _map_localeMsgInfos_codec);
            break;
          }
          case 88: {
            ManualEnd = input.ReadBool();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the Announcement message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static class Types {
      public enum MsgType {
        /// <summary>
        ///自定义
        /// </summary>
        Customize = 0,
        /// <summary>
        ///新餐厅
        /// </summary>
        NewRestaurant = 1,
        /// <summary>
        ///新活动
        /// </summary>
        NewEvent = 2,
        /// <summary>
        ///新功能
        /// </summary>
        NewFeature = 3,
      }

    }
    #endregion

  }

  public sealed class CListAnnouncements : pb::IMessage {
    private static readonly pb::MessageParser<CListAnnouncements> _parser = new pb::MessageParser<CListAnnouncements>(() => new CListAnnouncements());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CListAnnouncements> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  public sealed class SListAnnouncements : pb::IMessage {
    private static readonly pb::MessageParser<SListAnnouncements> _parser = new pb::MessageParser<SListAnnouncements>(() => new SListAnnouncements());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SListAnnouncements> Parser { get { return _parser; } }

    /// <summary>Field number for the "announcement" field.</summary>
    public const int AnnouncementFieldNumber = 1;
    private static readonly pb::FieldCodec<global::DragonU3DSDK.Network.API.ILProtocol.Announcement> _repeated_announcement_codec
        = pb::FieldCodec.ForMessage(10, global::DragonU3DSDK.Network.API.ILProtocol.Announcement.Parser);
    private readonly pbc::RepeatedField<global::DragonU3DSDK.Network.API.ILProtocol.Announcement> announcement_ = new pbc::RepeatedField<global::DragonU3DSDK.Network.API.ILProtocol.Announcement>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::DragonU3DSDK.Network.API.ILProtocol.Announcement> Announcement {
      get { return announcement_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      announcement_.WriteTo(output, _repeated_announcement_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += announcement_.CalculateSize(_repeated_announcement_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            announcement_.AddEntriesFrom(input, _repeated_announcement_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
