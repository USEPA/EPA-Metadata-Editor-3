<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:res="http://www.esri.com/metadata/res/" xmlns:esri="http://www.esri.com/metadata/" xmlns:msxsl="urn:schemas-microsoft-com:xslt" >

  <!-- An XSLT template for displaying metadata in ArcGIS that is stored in the ArcGIS metadata format.

     Copyright (c) 2009-2011, Environmental Systems Research Institute, Inc. All rights reserved.
	
     Revision History: Created 11/19/2009 avienneau
-->

  <xsl:template name="iteminfo" >
    <div class="itemDescription" id="overview">

	<!-- Title -->
	<!--
	<h1 class="idHeading">
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
	</h1>
	-->

	<h1 class="idHeading">
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/citation/citeinfo/title/text()">
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
	<div class="center">
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
	</div>

	<!-- Tags/Metadata Theme Keywords -->
	<p class="center">
		<span class="idHeading">Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/theme/themekey/text()">
				<xsl:for-each select="/metadata/idinfo/keywords/theme/themekey[text()]">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent">None</span>
			</xsl:otherwise>
		</xsl:choose>
		<br/>
		<span class="idHeading">Location Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/place/placekey/text()">
				<xsl:for-each select="/metadata/idinfo/keywords/place/placekey[text()]">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent">None</span>
			</xsl:otherwise>
		</xsl:choose>
   		<br/>
		<span class="idHeading">Temporal Keywords</span>
		<br/>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/keywords/temporal/tempkey/text()">
				<xsl:for-each select="/metadata/idinfo/keywords/temporal/tempkey[text()]">
					<xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent">None</span>
			</xsl:otherwise>
		</xsl:choose>
	</p><br/>

	<!-- AGOL Summary/Metadata Purpose -->
	<div class="itemInfo">
		<span class="idHeading">Abstract</span>
		<xsl:choose>
			<xsl:when test="(/metadata/idinfo/descript/abstract != '')">
				<p>
					<xsl:call-template name="elementSupportingMarkup">
						<xsl:with-param name="ele" select="/metadata/idinfo/descript/abstract" />
					</xsl:call-template>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p><span class="noContent">None</span></p>
			</xsl:otherwise>
		</xsl:choose>
	</div>

	<!-- AGOL Description/Metadata Abstract/Tool Summary -->
	<div class="itemInfo">
		<span class="idHeading"><res:idDesc_ItemDescription /></span>
		<xsl:choose>
			<xsl:when test="(/metadata/idinfo/descript/purpose != '')">
				<p>
					<xsl:call-template name="elementSupportingMarkup">
						<xsl:with-param name="ele" select="/metadata/idinfo/descript/purpose" />
					</xsl:call-template>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p><span class="noContent">None</span></p>
			</xsl:otherwise>
		</xsl:choose>
	</div>



	<!-- Physical Storage -->
	<div class="itemInfo">
		<span class="idHeading">Physical Storage</span>
		<p>
		<xsl:choose>
			<xsl:when test="/metadata/idinfo/citation/citeinfo/onlink/text()">
				<xsl:for-each select="/metadata/idinfo/citation/citeinfo/onlink[text()]">
					<xsl:value-of select="."/>
					<br/>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<span class="noContent">None</span>
			</xsl:otherwise>
		</xsl:choose>
		</p>
	</div>





	<!-- Credits -->
	<div class="itemInfo">
		<span class="idHeading"><res:idCredits_ItemDescription /></span>
		<xsl:choose>
			<xsl:when test="(/metadata/dataIdInfo[1]/idCredit != '')">
				<p>
					<xsl:call-template name="elementSupportingMarkup">
						<xsl:with-param name="ele" select="/metadata/dataIdInfo[1]/idCredit" />
					</xsl:call-template>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p><span class="noContent">None</span></p>
			</xsl:otherwise>
		</xsl:choose>
	</div>

	<!-- Publication Date -->
	<div class="itemInfo">
		<span class="idHeading">Publication Date</span>
		<xsl:choose>
			<xsl:when test="(/metadata/idinfo/citation/citeinfo/pubdate != '')">
				<p>
					<xsl:call-template name="elementSupportingMarkup">
						<xsl:with-param name="ele" select="/metadata/idinfo/citation/citeinfo/pubdate" />
					</xsl:call-template>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p><span class="noContent">None</span></p>
			</xsl:otherwise>
		</xsl:choose>
	</div>

	<!-- Use constraints -->
	<div class="itemInfo">
		<span class="idHeading"><res:idUseLimits_ItemDescription /></span>
		<xsl:choose>
			<xsl:when test="(/metadata/dataIdInfo[1]/resConst/Consts/useLimit[1] != '')">
				<p>
					<xsl:call-template name="elementSupportingMarkup">
						<xsl:with-param name="ele" select="/metadata/dataIdInfo[1]/resConst/Consts/useLimit[1]" />
					</xsl:call-template>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p><span class="noContent">None</span></p>
			</xsl:otherwise>
		</xsl:choose>
	</div>
	
	</div>
  </xsl:template>
  
  <xsl:template name="elementSupportingMarkup">
	<xsl:param name="ele" />
	<xsl:choose>
		<xsl:when test="$ele/*">
			<xsl:copy-of select="$ele/node()" />
		</xsl:when>
		<xsl:when test="$ele[(contains(.,'&lt;/')) or (contains(.,'/&gt;'))]">
			<xsl:variable name="text">
				<xsl:copy-of select="esri:decodenodeset($ele)" />
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
						<xsl:value-of select="esri:striphtml($ele)" />
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="($escapedHtmlText != '')">
							<p><xsl:call-template name="handleURLs">
								<xsl:with-param name="text" select="normalize-space($escapedHtmlText)" />
							</xsl:call-template></p>
						</xsl:when>
						<xsl:otherwise>
							<p><span class="noContent">
								<xsl:choose>
									<xsl:when test="(name($ele) = 'idAbs')"><res:idNoDescForItem/></xsl:when>
									<xsl:when test="(name($ele) = 'idPurp')"><res:idNoSummaryForItem/></xsl:when>
									<xsl:when test="(name($ele) = 'idCredit')"><res:idNoCreditsForItem/></xsl:when>
									<xsl:when test="(name($ele) = 'useLimit')"><res:idNoUseLimitsForItem/></xsl:when>
								</xsl:choose>
							</span></p>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$ele[(contains(.,'&amp;')) or (contains(.,'&lt;')) or (contains(.,'&gt;'))]">
			<xsl:call-template name="handleURLs">
				<xsl:with-param name="text" select="$ele" />
			</xsl:call-template>
		</xsl:when>
		<xsl:when test="$ele/text()">
			<xsl:variable name="escapedHtmlText">
				<xsl:value-of select="esri:striphtml($ele)" />
			</xsl:variable>
			<p><xsl:call-template name="handleURLs">
				<xsl:with-param name="text" select="normalize-space($escapedHtmlText)" />
			</xsl:call-template></p>
		</xsl:when>
	</xsl:choose>
  </xsl:template>
  
  <xsl:template name="extentTable">
	<xsl:param name="west" />
	<xsl:param name="east" />
	<xsl:param name="north" />
	<xsl:param name="south" />
	<dl>
		<dt>
			<table cols="4" width="auto" border="0">
				<col align="left" />
				<col align="right" />
				<col align="left" />
				<col align="right" />
				<tr>
					<td class="description"><span class="idHeading"><res:idExtentWest_ItemDescription /></span>&#160;</td>
					<td class="description"><xsl:value-of select="$west"/></td>
					<td class="description">&#160;&#160;&#160;<span class="idHeading"><res:idExtentEast_ItemDescription /></span>&#160;</td>
					<td class="description"><xsl:value-of select="$east"/></td>
				</tr>
				<tr>
					<td class="description"><span class="idHeading"><res:idExtentNorth_ItemDescription /></span>&#160;</td>
					<td class="description"><xsl:value-of select="$north"/></td>
					<td class="description">&#160;&#160;&#160;<span class="idHeading"><res:idExtentSouth_ItemDescription /></span>&#160;</td>
					<td class="description"><xsl:value-of select="$south"/></td>
				</tr>
			</table>
		</dt>
	</dl>
  </xsl:template>
  
</xsl:stylesheet>
