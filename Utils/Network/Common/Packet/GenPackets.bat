start ../../PacketGenerator/bin/PacketGenerator.exe ../../../../Assets/Scripts/Network/PDL.xml

timeout 1

xcopy /Y GenPackets.cs "../../Server/Packet"
xcopy /Y ServerPacketManager.cs "../../Server/Packet"

xcopy /Y GenPackets.cs "../../../../Assets/Scripts/Network/Packet"
xcopy /Y ClientPacketManager.cs "../../../../Assets/Scripts/Network/Packet"
