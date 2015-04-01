using System;

namespace FuseExample
{

	public class cEntityInfo
	{
		public cEntityInfo()
		{}

		public cEntityInfo(bool pIsInvalid)
		{
			this.IsInvalid = pIsInvalid;
		}


		public bool IsInvalid = false;
		public bool IsFolder = false;
		public bool IsLink = false;
	}


	public class cHandleStatusInfo
	{
		public ulong FileHandle;
		protected long m_Size;

		public long Size
		{
			get
			{ 
				return System.Text.Encoding.UTF8.GetBytes ("Hello World\n").LongLength;
			}

			set
			{ 
				m_Size = value;
			}
		}

		public uint OwnerId=0;
		public uint OwnerPrimaryGroupId= 0;

		public string PermissionString;

	}


}

