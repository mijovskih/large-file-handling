A .NET 10 console application that copies a file to a destination folder in chunks. The chunk size is fixed,
so memory stays flat regardless of file size.
I focused on OOP concepts and SOLID principles, with a clean and layered structure. The copy logic depends
entirely on interfaces. The concrete classes are wired up in Program.cs, and the reader and receiver are 
built via factories, since their paths are only known at runtime.

Added background functionality to hash chunks using MD5, and verify hash on transfer, after saving to hard disk.

Added functionality to show chunk checksums.

Changed from synchronous programming to asynchronous.
The console app would not benefit from asynchronous programming - it is only 1 request that copies a file, there are no additional requests that would be blocked if the thread is locked waiting for I/O. The real reason is future scalability. If we want to build an API on top of this application, the business logic needs to be re-usable and support such implementation. That's where we can really benefit from asynchronous programming.

Added functionality to compute and show whole file checksums.

Added functionality to display progress while file is being copied.

Added functionality to retry copying of chunk if for some reason it fails.