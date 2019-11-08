# sigma
Exercise project for Sigma IOT

Program is a ASP .Net Core 3.0 API project.
A background service fetches data from the blob and stores the data in memory.
By using polymorphism it is possible to add new sensor types to the project and encapsulating each sensor type with its own special functionality such as for example converting and returning temperature as fahrenheit.
