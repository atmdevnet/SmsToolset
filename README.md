# SmsToolset
## GSM SMS toolset (.net library)
SmsToolset is .net library written in C# which aims to help create applications of sending and receiving text messages in PDU format over GSM network.

The original key concept of this library is the PDU profile wchich is an object that determines structure of transmitted PDU packet. Basic part of PDU profile object is settings object driven by text file in well known JSON format. Parameters of profile defined in JSON file are loaded into settings object which initializes the PDU profile object. The essential task of profile is to create PDU packets. Another essential part of library is PDU profile manager that helps create and manage many different profiles.

In general the library consists of three main parts responsible for:
- creating and managing PDU profiles,
- creating commands for communications equipment (GSM modem),
- sending commands to and receiving response from communications equipment.

Library contains also basic implementation of tools that help with tasks of:
- authentication by pin,
- handling service center number,
- sending and reading text messages.

For more look at the article at codeproject.com: [SmsToolset library for sending sms over GSM network using PDU format](http://www.codeproject.com/Articles/1097367/SmsToolset-library-for-sending-sms-over-GSM-networ)

Nuget package targeting .NET Standard 2.0 is also available at: [SmsToolset](https://www.nuget.org/packages/SmsToolset/)
