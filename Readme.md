QRcode-Revit2013
----------------

Generate QRcodes within Revit 2013 using native Detail objects (filled regions) and group each instance together.

### Added code within to generate variable sized generic models as QRcode family instead of FilledRegions. See if you can find the code and uncomment, then build to create your own cool Q(a)R(t) Codes.

(<a href="http://sketchfab.com/show/aXiN39ZjBVlJObyOhsYGikDa3V8" title="View this on Sketchfab" target="_blank">click to view in 3D</a>)</h3>;<a href="http://sketchfab.com/show/aXiN39ZjBVlJObyOhsYGikDa3V8" title="qrcodeQ(a)R(t)-3.fbx" target="_blank"><div style="position: absolute; top: 140px; left: 183px; height: 82px; width: 82px; cursor: pointer; background: url(http://sketchfab.com/img/sketchfab-play.png) no-repeat;background-size: cover;"></div><a href="http://sketchfab.com/show/aXiN39ZjBVlJObyOhsYGikDa3V8" title="qrcodeQ(a)R(t)-3.fbx" target="_blank"><img src="http://sketchfab.com/urls/aXiN39ZjBVlJObyOhsYGikDa3V8/thumbnail_448.png?v=d7a1fe22aa1f545a58e246a04ffbeba0" alt="qrcodeQ(a)R(t)-3.fbx"/></a>

<img src="http://kylemorin.co/files/QR1.PNG">
<img src="http://kylemorin.co/files/QR2.jpg">

- Uses the ThoughtWorks QRcode library that is maintained at https://github.com/aaronogan/QR.NET

#### Known issue: FilledRegion generation is very slow. Still trying to optimize, but I think it is just the nature of Revit in creating this sort of object. Extracted out to create an annotation family then insert into project, sped up x2.
