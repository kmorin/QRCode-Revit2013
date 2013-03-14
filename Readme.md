QRcode-Revit2013
----------------

Generate QRcodes within Revit 2013 using native Detail objects (filled regions) and group each instance together.

### Added code within to generate variable sized generic models as QRcode family instead of FilledRegions. See if you can find the code and uncomment, then build to create your own cool Q(a)R(t) Codes.

<img src="https://www.dropbox.com/s/wy6c0ws3b09zbgw/QR1.PNG">
<img src="https://www.dropbox.com/s/310ftzauwwvxzc3/QR2.jpg">

- Uses the ThoughtWorks QRcode library that is maintained at https://github.com/aaronogan/QR.NET

#### Known issue: FilledRegion generation is very slow. Still trying to optimize, but I think it is just the nature of Revit in creating this sort of object. Extracted out to create an annotation family then insert into project, sped up x2.