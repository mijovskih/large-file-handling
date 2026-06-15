A .NET 10 console application that copies a file to a destination folder in chunks. The chunk size is fixed,
so memory stays flat regardless of file size.
I focused on OOP concepts and SOLID principles, with a clean and layered structure. The copy logic depends
entirely on interfaces. The concrete classes are wired up in Program.cs, and the reader and receiver are 
built via factories, since their paths are only known at runtime.

Additionally, the application supports each chunk being hashed after being written to the destination.
Verification is also done, where a retry policy would initiate if the chunk hashes do not match.
The chunk will be replaced with each retry.
If the retry policy exceeds the maximum amount of retry number, an exception is thrown.
In a bigger production based application, I would also consider deleting the destination file, since it would remain incomplete.
For now, I think this is enough. We might want to inspect the file after incomplete transfer.

During the copy, a progress bar is shown with a percentage status. After completion, we see the detailed information
regarding the chunks and their hashes, along with the hashes of the whole files.
The destination file hash is computed after the handle for the file is closed and disposed. This will prevent the possibility
of the hash being calculated before the file is finalized. Additional security note: I also set FileShare.None in the handle
for the destination file, meaning nothing will be able to read/write from the file until the handle is closed and disposed.

At first I began with synchronous programming, but then switched to asynchronous.
The console app would not benefit from asynchronous programming - it is only 1 request that copies a file, there are no additional requests that would be blocked if the thread is locked waiting for I/O. The real reason is future scalability. If we want to build an API on top of this application, the business logic needs to be re-usable and support such implementation. That's where we can really benefit from asynchronous programming.