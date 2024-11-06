<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">
		<html>
			<head>
				<style>
					table.doc {
						margin: 0;
						padding: 0;
						border-collapse: collapse;
					}
					
					table.doc tr:nth-of-type(odd) {
						background: #eee;
					}
					
					table.doc tr th {
						background: rgb(0, 160, 201);
						color: white;
					}
					
					table.doc td,table.doc th {
						padding: 6px;
						border: 1px solid #ccc;
						text-align: left;
					}	
				</style>
			</head>
			<body>
				<h2>ΠΑΡΑΣΤΑΤΙΚΑ</h2>
				<table class="doc">
					<tr>
						<th>ΑΡΙΘΜΟΣ</th>
						<th>ΕΙΔΗ</th>
						<th>ΔΗΜΙΟΥΡΓΗΘΗΚΕ</th>
						<th>ΚΑΤΑΣΤΑΣΗ</th>
						<th>ΤΥΠΟΣ</th>
						<th>ΕΚΠΤΩΣΗ</th>
						<th>ΣΥΝΟΛΟ</th>
						<th>ΕΛΕΓΧΘΗΚΕ</th>
						<th>ΕΚΤΕΛΕΣΤΗΚΕ</th>
						<th>ΟΡΙΣΤΙΚΟΠΟΙΗΘΗΚΕ</th>
						<th>ΤΕΛΙΚΟ ΣΥΝΟΛΟ</th>
					</tr>
					
					<!-- Node Names of node Document:
						Number
						TotalItems
						CreatedOn
						Status
						Type
						Discount
						HasBeenChecked
						HasBeenExecuted
						FinalizedDateTime
						Total
					 -->

					<xsl:for-each select="Documents/*">
						<tr>
							<xsl:for-each select="*">
								<td>
									<xsl:value-of select="." />
								</td>
							</xsl:for-each>
						</tr>
					</xsl:for-each>

				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>