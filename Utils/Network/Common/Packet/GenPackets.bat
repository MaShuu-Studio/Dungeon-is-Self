start ../../PacketGenerator/bin/PacketGenerator.exe ../../../../Assets/Scripts/Network/PDL.xml

timeout 1

xcopy /Y GenPackets.cs "../../Server/Packet"
xcopy /Y ServerPacketManager.cs "../../Server/Packet"

xcopy /Y GenPackets.cs "D:\Users\Unity\2D\Dungeon-is-Self\Assets\Scripts\Network\Packet"
xcopy /Y ClientPacketManager.cs "D:\Users\Unity\2D\Dungeon-is-Self\Assets\Scripts\Network\Packet"
