﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class PersonAlreadyExistException : Exception
	{
		public PersonAlreadyExistException() : base("Project member already added!")
		{

		}
	}
}
