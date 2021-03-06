﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Database
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Database")]
	public partial class LinqToSqlDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    partial void InsertFriend(Friendship instance);
    partial void UpdateFriend(Friendship instance);
    partial void DeleteFriend(Friendship instance);
    partial void InsertConversation(Conversation instance);
    partial void UpdateConversation(Conversation instance);
    partial void DeleteConversation(Conversation instance);
    partial void InsertConversation_reply(ConversationReply instance);
    partial void UpdateConversation_reply(ConversationReply instance);
    partial void DeleteConversation_reply(ConversationReply instance);
    partial void InsertNotification(Notification instance);
    partial void UpdateNotification(Notification instance);
    partial void DeleteNotification(Notification instance);
    #endregion
		
		public LinqToSqlDataContext() : 
				base(global::Server.Properties.Settings.Default.DatabaseConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToSqlDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToSqlDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToSqlDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToSqlDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<User> Users
		{
			get
			{
				return this.GetTable<User>();
			}
		}
		
		public System.Data.Linq.Table<Friendship> Friends
		{
			get
			{
				return this.GetTable<Friendship>();
			}
		}
		
		public System.Data.Linq.Table<Conversation> Conversations
		{
			get
			{
				return this.GetTable<Conversation>();
			}
		}
		
		public System.Data.Linq.Table<ConversationReply> Conversation_replies
		{
			get
			{
				return this.GetTable<ConversationReply>();
			}
		}
		
		public System.Data.Linq.Table<Notification> Notifications
		{
			get
			{
				return this.GetTable<Notification>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Users")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _user_id;
		
		private string _login;
		
		private string _password;
		
		private EntitySet<Friendship> _Friends;
		
		private EntitySet<Friendship> _Friends1;
		
		private EntitySet<Conversation> _Conversations;
		
		private EntitySet<Conversation> _Conversations1;
		
		private EntitySet<ConversationReply> _Conversation_replies;
		
		private EntitySet<Notification> _Notifications;
		
		private EntitySet<Notification> _Notifications1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onuser_idChanging(int value);
    partial void Onuser_idChanged();
    partial void OnloginChanging(string value);
    partial void OnloginChanged();
    partial void OnpasswordChanging(string value);
    partial void OnpasswordChanged();
    #endregion
		
		public User()
		{
			this._Friends = new EntitySet<Friendship>(new Action<Friendship>(this.attach_Friends), new Action<Friendship>(this.detach_Friends));
			this._Friends1 = new EntitySet<Friendship>(new Action<Friendship>(this.attach_Friends1), new Action<Friendship>(this.detach_Friends1));
			this._Conversations = new EntitySet<Conversation>(new Action<Conversation>(this.attach_Conversations), new Action<Conversation>(this.detach_Conversations));
			this._Conversations1 = new EntitySet<Conversation>(new Action<Conversation>(this.attach_Conversations1), new Action<Conversation>(this.detach_Conversations1));
			this._Conversation_replies = new EntitySet<ConversationReply>(new Action<ConversationReply>(this.attach_Conversation_replies), new Action<ConversationReply>(this.detach_Conversation_replies));
			this._Notifications = new EntitySet<Notification>(new Action<Notification>(this.attach_Notifications), new Action<Notification>(this.detach_Notifications));
			this._Notifications1 = new EntitySet<Notification>(new Action<Notification>(this.attach_Notifications1), new Action<Notification>(this.detach_Notifications1));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int user_id
		{
			get
			{
				return this._user_id;
			}
			set
			{
				if ((this._user_id != value))
				{
					this.Onuser_idChanging(value);
					this.SendPropertyChanging();
					this._user_id = value;
					this.SendPropertyChanged("user_id");
					this.Onuser_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_login", DbType="VarChar(30) NOT NULL", CanBeNull=false)]
		public string login
		{
			get
			{
				return this._login;
			}
			set
			{
				if ((this._login != value))
				{
					this.OnloginChanging(value);
					this.SendPropertyChanging();
					this._login = value;
					this.SendPropertyChanged("login");
					this.OnloginChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_password", DbType="VarChar(70) NOT NULL", CanBeNull=false)]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if ((this._password != value))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend", Storage="_Friends", ThisKey="user_id", OtherKey="friend_one")]
		public EntitySet<Friendship> Friends
		{
			get
			{
				return this._Friends;
			}
			set
			{
				this._Friends.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend1", Storage="_Friends1", ThisKey="user_id", OtherKey="friend_two")]
		public EntitySet<Friendship> Friends1
		{
			get
			{
				return this._Friends1;
			}
			set
			{
				this._Friends1.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation", Storage="_Conversations", ThisKey="user_id", OtherKey="user_one")]
		public EntitySet<Conversation> Conversations
		{
			get
			{
				return this._Conversations;
			}
			set
			{
				this._Conversations.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation1", Storage="_Conversations1", ThisKey="user_id", OtherKey="user_two")]
		public EntitySet<Conversation> Conversations1
		{
			get
			{
				return this._Conversations1;
			}
			set
			{
				this._Conversations1.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation_reply", Storage="_Conversation_replies", ThisKey="user_id", OtherKey="user_id")]
		public EntitySet<ConversationReply> Conversation_replies
		{
			get
			{
				return this._Conversation_replies;
			}
			set
			{
				this._Conversation_replies.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Notification", Storage="_Notifications", ThisKey="user_id", OtherKey="user_one")]
		public EntitySet<Notification> Notifications
		{
			get
			{
				return this._Notifications;
			}
			set
			{
				this._Notifications.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Notification1", Storage="_Notifications1", ThisKey="user_id", OtherKey="user_two")]
		public EntitySet<Notification> Notifications1
		{
			get
			{
				return this._Notifications1;
			}
			set
			{
				this._Notifications1.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Friends(Friendship entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Friends(Friendship entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Friends1(Friendship entity)
		{
			this.SendPropertyChanging();
			entity.User1 = this;
		}
		
		private void detach_Friends1(Friendship entity)
		{
			this.SendPropertyChanging();
			entity.User1 = null;
		}
		
		private void attach_Conversations(Conversation entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Conversations(Conversation entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Conversations1(Conversation entity)
		{
			this.SendPropertyChanging();
			entity.User1 = this;
		}
		
		private void detach_Conversations1(Conversation entity)
		{
			this.SendPropertyChanging();
			entity.User1 = null;
		}
		
		private void attach_Conversation_replies(ConversationReply entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Conversation_replies(ConversationReply entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Notifications(Notification entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_Notifications(Notification entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
		
		private void attach_Notifications1(Notification entity)
		{
			this.SendPropertyChanging();
			entity.User1 = this;
		}
		
		private void detach_Notifications1(Notification entity)
		{
			this.SendPropertyChanging();
			entity.User1 = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Friends")]
	public partial class Friendship : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _friend_one;
		
		private int _friend_two;
		
		private bool _accepted;
		
		private EntityRef<User> _User;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onfriend_oneChanging(int value);
    partial void Onfriend_oneChanged();
    partial void Onfriend_twoChanging(int value);
    partial void Onfriend_twoChanged();
    partial void OnacceptedChanging(bool value);
    partial void OnacceptedChanged();
    #endregion
		
		public Friendship()
		{
			this._User = default(EntityRef<User>);
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_friend_one", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int friend_one
		{
			get
			{
				return this._friend_one;
			}
			set
			{
				if ((this._friend_one != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onfriend_oneChanging(value);
					this.SendPropertyChanging();
					this._friend_one = value;
					this.SendPropertyChanged("friend_one");
					this.Onfriend_oneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_friend_two", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int friend_two
		{
			get
			{
				return this._friend_two;
			}
			set
			{
				if ((this._friend_two != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onfriend_twoChanging(value);
					this.SendPropertyChanging();
					this._friend_two = value;
					this.SendPropertyChanged("friend_two");
					this.Onfriend_twoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_accepted", DbType="Bit NOT NULL")]
		public bool accepted
		{
			get
			{
				return this._accepted;
			}
			set
			{
				if ((this._accepted != value))
				{
					this.OnacceptedChanging(value);
					this.SendPropertyChanging();
					this._accepted = value;
					this.SendPropertyChanged("accepted");
					this.OnacceptedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend", Storage="_User", ThisKey="friend_one", OtherKey="user_id", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Friends.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Friends.Add(this);
						this._friend_one = value.user_id;
					}
					else
					{
						this._friend_one = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Friend1", Storage="_User1", ThisKey="friend_two", OtherKey="user_id", IsForeignKey=true)]
		public User User1
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Friends1.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Friends1.Add(this);
						this._friend_two = value.user_id;
					}
					else
					{
						this._friend_two = default(int);
					}
					this.SendPropertyChanged("User1");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Conversations")]
	public partial class Conversation : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _conversation_id;
		
		private int _user_one;
		
		private int _user_two;
		
		private EntitySet<ConversationReply> _Conversation_replies;
		
		private EntityRef<User> _User;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onconversation_idChanging(int value);
    partial void Onconversation_idChanged();
    partial void Onuser_oneChanging(int value);
    partial void Onuser_oneChanged();
    partial void Onuser_twoChanging(int value);
    partial void Onuser_twoChanged();
    #endregion
		
		public Conversation()
		{
			this._Conversation_replies = new EntitySet<ConversationReply>(new Action<ConversationReply>(this.attach_Conversation_replies), new Action<ConversationReply>(this.detach_Conversation_replies));
			this._User = default(EntityRef<User>);
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_conversation_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int conversation_id
		{
			get
			{
				return this._conversation_id;
			}
			set
			{
				if ((this._conversation_id != value))
				{
					this.Onconversation_idChanging(value);
					this.SendPropertyChanging();
					this._conversation_id = value;
					this.SendPropertyChanged("conversation_id");
					this.Onconversation_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_one", DbType="Int NOT NULL")]
		public int user_one
		{
			get
			{
				return this._user_one;
			}
			set
			{
				if ((this._user_one != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onuser_oneChanging(value);
					this.SendPropertyChanging();
					this._user_one = value;
					this.SendPropertyChanged("user_one");
					this.Onuser_oneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_two", DbType="Int NOT NULL")]
		public int user_two
		{
			get
			{
				return this._user_two;
			}
			set
			{
				if ((this._user_two != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onuser_twoChanging(value);
					this.SendPropertyChanging();
					this._user_two = value;
					this.SendPropertyChanged("user_two");
					this.Onuser_twoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Conversation_Conversation_reply", Storage="_Conversation_replies", ThisKey="conversation_id", OtherKey="conversation_id")]
		public EntitySet<ConversationReply> Conversation_replies
		{
			get
			{
				return this._Conversation_replies;
			}
			set
			{
				this._Conversation_replies.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation", Storage="_User", ThisKey="user_one", OtherKey="user_id", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Conversations.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Conversations.Add(this);
						this._user_one = value.user_id;
					}
					else
					{
						this._user_one = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation1", Storage="_User1", ThisKey="user_two", OtherKey="user_id", IsForeignKey=true)]
		public User User1
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Conversations1.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Conversations1.Add(this);
						this._user_two = value.user_id;
					}
					else
					{
						this._user_two = default(int);
					}
					this.SendPropertyChanged("User1");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Conversation_replies(ConversationReply entity)
		{
			this.SendPropertyChanging();
			entity.Conversation = this;
		}
		
		private void detach_Conversation_replies(ConversationReply entity)
		{
			this.SendPropertyChanging();
			entity.Conversation = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Conversation_replies")]
	public partial class ConversationReply : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _reply_id;
		
		private string _reply;
		
		private int _conversation_id;
		
		private int _user_id;
		
		private System.DateTime _time;
		
		private EntityRef<Conversation> _Conversation;
		
		private EntityRef<User> _User;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onreply_idChanging(int value);
    partial void Onreply_idChanged();
    partial void OnreplyChanging(string value);
    partial void OnreplyChanged();
    partial void Onconversation_idChanging(int value);
    partial void Onconversation_idChanged();
    partial void Onuser_idChanging(int value);
    partial void Onuser_idChanged();
    partial void OntimeChanging(System.DateTime value);
    partial void OntimeChanged();
    #endregion
		
		public ConversationReply()
		{
			this._Conversation = default(EntityRef<Conversation>);
			this._User = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_reply_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int reply_id
		{
			get
			{
				return this._reply_id;
			}
			set
			{
				if ((this._reply_id != value))
				{
					this.Onreply_idChanging(value);
					this.SendPropertyChanging();
					this._reply_id = value;
					this.SendPropertyChanged("reply_id");
					this.Onreply_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_reply", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string reply
		{
			get
			{
				return this._reply;
			}
			set
			{
				if ((this._reply != value))
				{
					this.OnreplyChanging(value);
					this.SendPropertyChanging();
					this._reply = value;
					this.SendPropertyChanged("reply");
					this.OnreplyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_conversation_id", DbType="Int NOT NULL")]
		public int conversation_id
		{
			get
			{
				return this._conversation_id;
			}
			set
			{
				if ((this._conversation_id != value))
				{
					if (this._Conversation.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onconversation_idChanging(value);
					this.SendPropertyChanging();
					this._conversation_id = value;
					this.SendPropertyChanged("conversation_id");
					this.Onconversation_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_id", DbType="Int NOT NULL")]
		public int user_id
		{
			get
			{
				return this._user_id;
			}
			set
			{
				if ((this._user_id != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onuser_idChanging(value);
					this.SendPropertyChanging();
					this._user_id = value;
					this.SendPropertyChanged("user_id");
					this.Onuser_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_time", DbType="DateTime2 NOT NULL")]
		public System.DateTime time
		{
			get
			{
				return this._time;
			}
			set
			{
				if ((this._time != value))
				{
					this.OntimeChanging(value);
					this.SendPropertyChanging();
					this._time = value;
					this.SendPropertyChanged("time");
					this.OntimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Conversation_Conversation_reply", Storage="_Conversation", ThisKey="conversation_id", OtherKey="conversation_id", IsForeignKey=true)]
		public Conversation Conversation
		{
			get
			{
				return this._Conversation.Entity;
			}
			set
			{
				Conversation previousValue = this._Conversation.Entity;
				if (((previousValue != value) 
							|| (this._Conversation.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Conversation.Entity = null;
						previousValue.Conversation_replies.Remove(this);
					}
					this._Conversation.Entity = value;
					if ((value != null))
					{
						value.Conversation_replies.Add(this);
						this._conversation_id = value.conversation_id;
					}
					else
					{
						this._conversation_id = default(int);
					}
					this.SendPropertyChanged("Conversation");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Conversation_reply", Storage="_User", ThisKey="user_id", OtherKey="user_id", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Conversation_replies.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Conversation_replies.Add(this);
						this._user_id = value.user_id;
					}
					else
					{
						this._user_id = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Notifications")]
	public partial class Notification : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _notification_id;
		
		private int _user_one;
		
		private int _user_two;
		
		private System.DateTime _time;
		
		private bool _accept_friendship;
		
		private bool _reject_friendship;
		
		private bool _send_friendship;
		
		private bool _cancel_friendship;
		
		private bool _remove_friend;
		
		private EntityRef<User> _User;
		
		private EntityRef<User> _User1;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onnotification_idChanging(int value);
    partial void Onnotification_idChanged();
    partial void Onuser_oneChanging(int value);
    partial void Onuser_oneChanged();
    partial void Onuser_twoChanging(int value);
    partial void Onuser_twoChanged();
    partial void OntimeChanging(System.DateTime value);
    partial void OntimeChanged();
    partial void Onaccept_friendshipChanging(bool value);
    partial void Onaccept_friendshipChanged();
    partial void Onreject_friendshipChanging(bool value);
    partial void Onreject_friendshipChanged();
    partial void Onsend_friendshipChanging(bool value);
    partial void Onsend_friendshipChanged();
    partial void Oncancel_friendshipChanging(bool value);
    partial void Oncancel_friendshipChanged();
    partial void Onremove_friendChanging(bool value);
    partial void Onremove_friendChanged();
    #endregion
		
		public Notification()
		{
			this._User = default(EntityRef<User>);
			this._User1 = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_notification_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int notification_id
		{
			get
			{
				return this._notification_id;
			}
			set
			{
				if ((this._notification_id != value))
				{
					this.Onnotification_idChanging(value);
					this.SendPropertyChanging();
					this._notification_id = value;
					this.SendPropertyChanged("notification_id");
					this.Onnotification_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_one", DbType="Int NOT NULL")]
		public int user_one
		{
			get
			{
				return this._user_one;
			}
			set
			{
				if ((this._user_one != value))
				{
					if (this._User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onuser_oneChanging(value);
					this.SendPropertyChanging();
					this._user_one = value;
					this.SendPropertyChanged("user_one");
					this.Onuser_oneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_user_two", DbType="Int NOT NULL")]
		public int user_two
		{
			get
			{
				return this._user_two;
			}
			set
			{
				if ((this._user_two != value))
				{
					if (this._User1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.Onuser_twoChanging(value);
					this.SendPropertyChanging();
					this._user_two = value;
					this.SendPropertyChanged("user_two");
					this.Onuser_twoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_time", DbType="DateTime2 NOT NULL")]
		public System.DateTime time
		{
			get
			{
				return this._time;
			}
			set
			{
				if ((this._time != value))
				{
					this.OntimeChanging(value);
					this.SendPropertyChanging();
					this._time = value;
					this.SendPropertyChanged("time");
					this.OntimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_accept_friendship", DbType="Bit NOT NULL")]
		public bool accept_friendship
		{
			get
			{
				return this._accept_friendship;
			}
			set
			{
				if ((this._accept_friendship != value))
				{
					this.Onaccept_friendshipChanging(value);
					this.SendPropertyChanging();
					this._accept_friendship = value;
					this.SendPropertyChanged("accept_friendship");
					this.Onaccept_friendshipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_reject_friendship", DbType="Bit NOT NULL")]
		public bool reject_friendship
		{
			get
			{
				return this._reject_friendship;
			}
			set
			{
				if ((this._reject_friendship != value))
				{
					this.Onreject_friendshipChanging(value);
					this.SendPropertyChanging();
					this._reject_friendship = value;
					this.SendPropertyChanged("reject_friendship");
					this.Onreject_friendshipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_send_friendship", DbType="Bit NOT NULL")]
		public bool send_friendship
		{
			get
			{
				return this._send_friendship;
			}
			set
			{
				if ((this._send_friendship != value))
				{
					this.Onsend_friendshipChanging(value);
					this.SendPropertyChanging();
					this._send_friendship = value;
					this.SendPropertyChanged("send_friendship");
					this.Onsend_friendshipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_cancel_friendship", DbType="Bit NOT NULL")]
		public bool cancel_friendship
		{
			get
			{
				return this._cancel_friendship;
			}
			set
			{
				if ((this._cancel_friendship != value))
				{
					this.Oncancel_friendshipChanging(value);
					this.SendPropertyChanging();
					this._cancel_friendship = value;
					this.SendPropertyChanged("cancel_friendship");
					this.Oncancel_friendshipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_remove_friend", DbType="Bit NOT NULL")]
		public bool remove_friend
		{
			get
			{
				return this._remove_friend;
			}
			set
			{
				if ((this._remove_friend != value))
				{
					this.Onremove_friendChanging(value);
					this.SendPropertyChanging();
					this._remove_friend = value;
					this.SendPropertyChanged("remove_friend");
					this.Onremove_friendChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Notification", Storage="_User", ThisKey="user_one", OtherKey="user_id", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._User.Entity;
			}
			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User.Entity = null;
						previousValue.Notifications.Remove(this);
					}
					this._User.Entity = value;
					if ((value != null))
					{
						value.Notifications.Add(this);
						this._user_one = value.user_id;
					}
					else
					{
						this._user_one = default(int);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_Notification1", Storage="_User1", ThisKey="user_two", OtherKey="user_id", IsForeignKey=true)]
		public User User1
		{
			get
			{
				return this._User1.Entity;
			}
			set
			{
				User previousValue = this._User1.Entity;
				if (((previousValue != value) 
							|| (this._User1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._User1.Entity = null;
						previousValue.Notifications1.Remove(this);
					}
					this._User1.Entity = value;
					if ((value != null))
					{
						value.Notifications1.Add(this);
						this._user_two = value.user_id;
					}
					else
					{
						this._user_two = default(int);
					}
					this.SendPropertyChanged("User1");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
