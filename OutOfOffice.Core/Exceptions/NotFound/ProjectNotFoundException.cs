﻿using OutOfOffice.Core.Exceptions.NotFound.Base;

namespace OutOfOffice.Core.Exceptions.NotFound;

public class ProjectNotFoundException(string message) : BaseNotFoundException(message);