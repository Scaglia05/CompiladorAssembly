# Arquivo de teste para simulador MIPS

addi $t0, $zero, 8      # $t0 = 8
addi $t1, $zero, 4      # $t1 = 4
add  $t2, $t0, $t1      # $t2 = 12
sub  $t3, $t0, $t1      # $t3 = 4
and  $t4, $t0, $t1      # $t4 = 0
or   $t5, $t0, $t1      # $t5 = 12
slt  $t6, $t1, $t0      # $t6 = 1
sw   $t2, 0($t3)        # Mem[$t3 + 0] = $t2
lw   $t7, 0($t3)        # $t7 = Mem[$t3 + 0]
beq  $t0, $t1, IGUAL    # se $t0 == $t1 salta
j    FIM                # Salta incondicionalmente

IGUAL:
addi $s0, $zero, 1      # $s0 = 1

FIM: