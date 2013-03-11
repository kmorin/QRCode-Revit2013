QRcode-Revit2013
----------------

Generate QRcodes within Revit 2013 using native Detail objects (filled regions) and group each instance together.

- Uses the ThoughtWorks QRcode library that is maintained at https://github.com/aaronogan/QR.NET

#### Known issue: FilledRegion generation is very slow. Still trying to optimize, but I think it is just the nature of Revit in creating this sort of object. Extracted out to create an annotation family then insert into project, sped up x2.