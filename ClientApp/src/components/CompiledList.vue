<script lang="ts">
    import DocumentationIcon from './icons/IconDocumentation.vue'
    import ToolingIcon from './icons/IconTooling.vue'
    import EcosystemIcon from './icons/IconEcosystem.vue'
    import CommunityIcon from './icons/IconCommunity.vue'
    import SupportIcon from './icons/IconSupport.vue'
    /*d efineProps< {
      la ngs: string[]
    } > ( )*/

    export default {
        //  props: ['langs'],
        data() {
            return {
                table: null,
                selected: null,
                collapsed: new Set(),
                style: ''
            }
        },
        mounted() {
            fetch('/list/compiled?urls=' + this.$route.query.l.join(';')).then(result => result.json()).then(
                json => {
                    this.table = json.table;
                    let pos = 0;
                    let mark = true;
                    let marked = [];
                    debugger;
                    for (var i = 0; i < json.widths.length; i++) {
                        if (mark) {
                            for (var j = 0; j < json.widths[i]; j++) {
                                marked.push(pos++);
                            }
                        } else {
                            pos += json.widths[i];
                        }
                        mark = !mark;
                    }
                    this.style = '<style>' + marked.map(_ => 'td:nth-child(' + (_ + 3) + ')').join(',') + '{ background: beige;} </style>';
                    console.log(this.style);
                }
            )
        },
        methods: {
            merge(index) {
                let src = this.table[this.selected];
                let dst = this.table[index];
                for (var i = 0; i < Math.min(dst.length, src.length); i++) {
                    dst[i] = dst[i] || src[i];
                }
                this.table.splice(this.selected, 1);
                this.selected = undefined;
            }
        }
    }

</script>

<template>
    <head v-html="style"/>
    <div v-if="table">
        Select rows to merge them, click column headers to (un)collapse
        <table>
            <tbody>
                <tr v-for="(row, index) in table">
                    <td>
                        <input type="checkbox" v-bind:checked="selected==index" @change="selected=selected?undefined:index" v-if="index && (!selected || selected == index)" />
                        <span class="plus" v-if="index && selected && selected != index" @click="merge(index)"> + </span>
                    </td>
                    <td v-for="(cell, index2) in row" @click="(!index) && ( (!collapsed.delete(index2)) && collapsed.add(index2) )">
                        <span v-bind:class="'cell ' + (collapsed.has(index2) ? 'collapsed' : '')">{{cell}}</span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div v-else>Loading</div>
</template>
<style scoped>
    table {
        border-collapse: collapse;
    }
    td {
        border: solid 1px grey;
    }
    td.odd{
        background: beige;
    }
    span.plus{
        cursor: default;
        user-select: none;
    }
    .cell{
        display:inline-block;
    }
    .cell.collapsed{
        width: 5px;
        overflow: hidden;
        opacity: 0;
    }
    input[type=checkbox], .plus{
        margin: 0 5px;
    }
</style>

