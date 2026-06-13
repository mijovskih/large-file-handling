A .NET 10 console application that copies a file to a destination folder in chunks. The chunk size is fixed,
so memory stays flat regardless of file size.
I focused on OOP concepts and SOLID principles, with a clean and layered structure. The copy logic depends
entirely on interfaces. The concrete classes are wired up in Program.cs, and the reader and receiver are 
built via factories, since their paths are only known at runtime.

Added background functionality to hash chunks using MD5, and verify hash on transfer, after saving to hard disk.

Added functionality to show chunk checksums.