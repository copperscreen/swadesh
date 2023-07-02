<script lang="ts">
    import DocumentationIcon from './icons/IconDocumentation.vue'
    import ToolingIcon from './icons/IconTooling.vue'
    import EcosystemIcon from './icons/IconEcosystem.vue'
    import CommunityIcon from './icons/IconCommunity.vue'
    import SupportIcon from './icons/IconSupport.vue'
    const empty : string[][] = [];
    export default {
        data() {
            return {
                table: empty,
                selected: -1,
                collapsed: new Set(),
                style: ''
            }
        },
        mounted() {
            //const url = '/list/compiled?urls=';
            const url = 'https://7pc236yso44fjuntl2mjcnu7nu0jiabs.lambda-url.us-east-1.on.aws/?urls=';
            const query = this.$route.query.l;
            if ((typeof(query) != 'string') && query?.length) {
                fetch(url + query.join('~')).then(result => result.json()).then(
                    json => {
                        this.table = json.table;
                        let pos = 0;
                        let mark = true;
                        let marked = [];
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
            } else {
                this.$router.push({ name: 'home' });
            }
        },
        methods: {
            merge(index : number) {
                let src = this.table[this.selected];
                let dst = this.table[index];
                for (var i = 0; i < Math.min(dst.length, src.length); i++) {
                    dst[i] = dst[i] || src[i];
                }
                this.table.splice(this.selected, 1);
                this.selected = -1;
            }
        }
    }

</script>

<template>
    <head v-html="style"/>
    <div v-if="table.length">
        Select rows to merge them, click column headers to (un)collapse. <a href="/">Click here to return to the language list</a>
        <table>
            <tbody>
                <tr v-for="(row, index) in table">
                    <td>
                        <input type="checkbox" v-bind:checked="selected==index" @change="selected=(selected==-1)?index:-1" v-if="index && ( selected < 0 || selected == index)" />
                        <span class="plus" v-if="index && selected >= 0 && selected != index" @click="merge(index)"> + </span>
                    </td>
                    <td v-for="(cell, index2) in row" @click="(!index) && ( (!collapsed.delete(index2)) && collapsed.add(index2) )">
                        <span v-bind:class="'cell ' + (collapsed.has(index2) ? 'collapsed' : '')">{{cell}}</span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div v-else class="waiting">Loading</div>
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

