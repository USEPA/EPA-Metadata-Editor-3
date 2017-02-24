<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:res="http://www.esri.com/metadata/res/" xmlns:esri="http://www.esri.com/metadata/" xmlns:msxsl="urn:schemas-microsoft-com:xslt" >

  <!-- An XSLT template for displaying metadata in ArcGIS that is stored in the ArcGIS metadata format.

     Copyright (c) 2009-2011, Environmental Systems Research Institute, Inc. All rights reserved.
	
     Revision History: Created 11/19/2009 avienneau
  -->

  <xsl:template name="iteminfo" >
    <div class="itemDescription" id="overview">

	<!-- Title -->

	<h1 class="idHeading">
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/citation/citeinfo/title">
				<xsl:value-of select="/metadata/idinfo/citation/citeinfo/title" />
			</xsl:when>
			<xsl:when test="/metadata/Esri/DataProperties/itemProps/itemName/text()">
				<xsl:value-of select="/metadata/Esri/DataProperties/itemProps/itemName" />
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent"><res:idNoTitle/></span>
			</xsl:otherwise>
		</xsl:choose>
	</h1>

	
	<!--<h1 class="idHeading">
		<xsl:choose>
			<xsl:when test="/metadata/dataIdInfo[1]/idCitation/resTitle/text()">
				<xsl:value-of select="/metadata/dataIdInfo[1]/idCitation/resTitle[1]" />
			</xsl:when>
			<xsl:when test="/metadata/Esri/DataProperties/itemProps/itemName/text()">
				<xsl:value-of select="/metadata/Esri/DataProperties/itemProps/itemName" />
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent"><res:idNoTitle/></span>
			</xsl:otherwise>
		</xsl:choose>
	</h1>-->

	<!-- Data type -->
	<xsl:if test="/metadata/distInfo/distFormat/formatName or /metadata/distInfo/distributor/distorFormat/formatName">
		<p class="center">
			<span class="idHeading">
				<xsl:choose>
					<xsl:when test="/metadata/distInfo/distFormat/formatName/text()">
						<xsl:value-of select="/metadata/distInfo/distFormat/formatName[text()][1]"/>
					</xsl:when>
					<xsl:when test="/metadata/distInfo/distributor/distorFormat/formatName/text()">
						<xsl:value-of select="/metadata/distInfo/distributor/distorFormat/formatName[text()][1]"/>
					</xsl:when>
				</xsl:choose>
			</span>
		</p>
	</xsl:if>

	<!-- Thumbnail/Tool Illustration -->
	<xsl:choose>
		<xsl:when test="/metadata/Binary/Thumbnail/img/@src">
			<img name="thumbnail" id="thumbnail" alt="Thumbnail" title="Thumbnail" class="summary">
				<xsl:attribute name="src">
					<xsl:value-of select="/metadata/Binary/Thumbnail/img/@src"/>
				</xsl:attribute>
				<xsl:attribute name="class">center</xsl:attribute>
			</img>
		</xsl:when>
		<xsl:when test="/metadata/idinfo/browse/img/@src">
			<img name="thumbnail" id="thumbnail" alt="Thumbnail" title="Thumbnail" class="summary">
				<xsl:attribute name="src">
					<xsl:value-of select="/metadata/idinfo/browse/img/@src"/>
				</xsl:attribute>
				<xsl:attribute name="class">center</xsl:attribute>
			</img>
		</xsl:when>
		<xsl:otherwise>
			<div class="noThumbnail"><res:idThumbnailNotAvail/></div>
		</xsl:otherwise>
	</xsl:choose>

	<!-- Tags/Metadata Theme Keywords -->
	<p class="center">
		<span class="idHeading">Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/theme">
				<xsl:for-each select="/metadata/idinfo/keywords/theme/themekey">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
		</xsl:choose>
		<br/>
		<span class="idHeading">Location Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/place">
				<xsl:for-each select="/metadata/idinfo/keywords/place/placekey">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
		</xsl:choose>
   		<br/>
		<span class="idHeading">Temporal Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/temporal">
				<xsl:for-each select="/metadata/idinfo/keywords/temporal/tempkey">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent"><res:idNoTagsForItem/></span>
			</xsl:otherwise>
		</xsl:choose>
		
	</p>

	<!-- AGOL Summary/Metadata Purpose -->
	<p>
		<span class="idHeading">Abstract</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/descript">
				<xsl:value-of select="/metadata/idinfo/descript/abstract" />
			</xsl:when>
	  	</xsl:choose>
	</p>
	
	<!-- AGOL Description/Metadata Abstract/Tool Summary -->
	<p>
		<span class="idHeading">Purpose</span>
		<br/>
		<xsl:choose>
		<xsl:when test="/metadata/idinfo/descript">
				<xsl:value-of select="/metadata/idinfo/descript/purpose" />
			</xsl:when>
	  	</xsl:choose>
	</p>

	<!-- Physical Storage -->
	<p>
		<span class="idHeading">Physical Storage</span>
		<br/>

		<xsl:choose>
			<xsl:when test="/metadata/idinfo/citation/citeinfo/onlink">
				<xsl:for-each select="/metadata/idinfo/citation/citeinfo/onlink">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())"><br/> </xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent">No Storage Info</span>
			</xsl:otherwise>
		</xsl:choose>
	</p>

	<!-- Data Set Credit -->
	<p>
		<span class="idHeading">Data Set Credit</span>
		<br/>

		<xsl:choose>
			<xsl:when test="/metadata/dataIdInfo[1]/idCredit/*">
				<xsl:copy-of select="/metadata/dataIdInfo[1]/idCredit/node()" />
			</xsl:when>
			<xsl:when test="/metadata/dataIdInfo[1]/idCredit[(contains(.,'&lt;/')) or (contains(.,'/&gt;'))]">
				<xsl:variable name="text">
					<xsl:copy-of select="esri:decodenodeset(/metadata/dataIdInfo[1]/idCredit)" />
				</xsl:variable>
				<xsl:choose>
					<xsl:when test="($text != '')">
						<xsl:variable name="newText">
							<xsl:apply-templates select="msxsl:node-set($text)/node() | msxsl:node-set($text)/@*" mode="linkTarget" />
						</xsl:variable>
						<xsl:copy-of select="$newText" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:variable name="escapedHtmlText">
							<xsl:value-of select="esri:striphtml(/metadata/dataIdInfo[1]/idCredit)" />
						</xsl:variable>
						<xsl:call-template name="handleURLs">
							<xsl:with-param name="text" select="normalize-space($escapedHtmlText)" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="/metadata/dataIdInfo[1]/idCredit[(contains(.,'&amp;')) or (contains(.,'&lt;')) or (contains(.,'&gt;'))]">
				<xsl:call-template name="handleURLs">
					<xsl:with-param name="text" select="/metadata/dataIdInfo[1]/idCredit" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="/metadata/dataIdInfo[1]/idCredit/text()">
				<xsl:variable name="escapedHtmlText">
					<xsl:value-of select="esri:striphtml(/metadata/dataIdInfo[1]/idCredit)" />
				</xsl:variable>
				<xsl:call-template name="handleURLs">
					<xsl:with-param name="text" select="normalize-space($escapedHtmlText)" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent"></span>
			</xsl:otherwise>
		</xsl:choose>
		
	</p>

	<!-- Publication Date -->
	<p>
		<span class="idHeading">Publication Date</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/citation/citeinfo">
				<xsl:value-of select="/metadata/idinfo/citation/citeinfo/pubdate" />
			</xsl:when>
	  	</xsl:choose>
		<xsl:choose>
			<xsl:when test="/metadata/dataIdInfo[1]/resConst/Consts/useLimit[1]/text()">
				<xsl:value-of select="/metadata/dataIdInfo[1]/resConstt/Consts/useLimit[1][text()]" />
			</xsl:when>
			
			<xsl:otherwise>
				<span class="noContent"></span>
			</xsl:otherwise>
		</xsl:choose>
		
	</p>

    </div>
  </xsl:template>
</xsl:stylesheet>
