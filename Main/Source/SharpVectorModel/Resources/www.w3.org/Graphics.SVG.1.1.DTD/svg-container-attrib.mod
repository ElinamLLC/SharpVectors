<!-- ....................................................................... -->
<!-- SVG 1.1 Container Attribute Module .................................... -->
<!-- file: svg-container-attrib.mod

     This is SVG, a language for describing two-dimensional graphics in XML.
     Copyright 2001, 2002 W3C (MIT, INRIA, Keio), All Rights Reserved.
     Revision: $Id: svg-container-attrib.mod,v 1.2 2002/04/20 18:07:42 fujisawa Exp $

     This DTD module is identified by the PUBLIC and SYSTEM identifiers:

        PUBLIC "-//W3C//ENTITIES SVG 1.1 Container Attribute//EN"
        SYSTEM "http://www.w3.org/Graphics/SVG/1.1/DTD/svg-container-attrib.mod"

     ....................................................................... -->

<!-- Container Attribute

        enable-background

     This module defines the Container attribute set.
-->

<!-- 'enable-background' property/attribute value (e.g., 'new', 'accumulate') -->
<!ENTITY % EnableBackgroundValue.datatype "CDATA" >

<!ENTITY % SVG.enable-background.attrib
    "enable-background %EnableBackgroundValue.datatype; #IMPLIED"
>

<!ENTITY % SVG.Container.extra.attrib "" >

<!ENTITY % SVG.Container.attrib
    "%SVG.enable-background.attrib;
     %SVG.Container.extra.attrib;"
>

<!-- end of svg-container-attrib.mod -->
